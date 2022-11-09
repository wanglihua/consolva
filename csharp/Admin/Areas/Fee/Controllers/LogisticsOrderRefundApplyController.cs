using Core;
using Core.Util;
using Domain;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Customer;
using Domain.LogisticsOrder;
using Domain.Pass;
using NHibernate.SqlCommand;
using PayPal.Api;
using Service;
using Service.Customer;
using Service.LogisticsOrder;
using Web.Areas.Admin.Models.LogisticsOrderRefundApply;
using Order = NHibernate.Criterion.Order;

namespace Web.Areas.Admin.Controllers
{
    [AdminArea]
    [AdminAuth(PassRes.OrderFee.RefundApply)]
    public class LogisticsOrderRefundApplyController : Controller
    {
        private ISession session = null;
        private CustomerService customerService = null;
        private LoginUserService loginUserService = null;
        private LogisticsOrderFeeReturnToCustomerService logisticsOrderFeeReturnToCustomerService;

        public LogisticsOrderRefundApplyController(
            ISession session,
            CustomerService customerService,
            LoginUserService loginUserService,
            LogisticsOrderFeeReturnToCustomerService logisticsOrderFeeReturnToCustomerService)
        {
            this.session = session;
            this.customerService = customerService;
            this.loginUserService = loginUserService;
            this.logisticsOrderFeeReturnToCustomerService = logisticsOrderFeeReturnToCustomerService;
        }

        [HttpGet]
        [Route("/admin/logistics/order/refund/apply")]
        [NoHibernateAutoTransaction]
        public IActionResult Index()
        {
            LogisticsOrderRefundApplyIndexModel model = new LogisticsOrderRefundApplyIndexModel();

            model.TypeList = LogisticsOrderRefundType.GetList();
            model.StatusList = LogisticsOrderRefundApplyStatus.GetList();
            model.ScopeList = ApplyDataViewScope.GetList();

            return View("LogisticsOrderRefundApplyIndex", model);
        }

        [HttpPost]
        [Route("/admin/logistics/order/refund/apply/list")]
        [NoHibernateAutoTransaction]
        public IActionResult List(LogisticsOrderRefundApplyListQueryParam queryParam)
        {
            int totalCount = GetListCriteria(queryParam)
                .SetProjection(Projections.RowCount())
                .UniqueResult<int>();

            ListPager listPager = new ListPager(queryParam, totalCount);

            IList<LogisticsOrderRefundApplyInfo> logisticsOrderRefundApplyList = GetListCriteria(queryParam)
                .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
                .AddOrder(Order.Desc(nameof(Model.Id)))
                .SetFirstResult(listPager.PageIndex * listPager.PageSize)
                .SetMaxResults(listPager.PageSize)
                .List<LogisticsOrderRefundApplyInfo>();

            IList<int> orderIdList = logisticsOrderRefundApplyList.Select(e => e.Order?.Id ?? 0).ToList();

            /*
            IList<LogisticsOrderRefundApplyCountInfo> applyCountList = session
                .Query<LogisticsOrderRefundApplyCountInfo>()
                .Where(e => orderIdList.Contains(e.OrderId))
                .ToList();

            foreach (LogisticsOrderRefundApplyInfo apply in logisticsOrderRefundApplyList)
            {
                int orderId = apply.Order?.Id ?? 00;
                apply.OrderApplyCountCache = applyCountList.Where(e => e.OrderId == orderId).Take(1).SingleOrDefault()?.ApplyCount ?? 0;
            }
            */

            IDictionary<int, int> dictApplyCount = logisticsOrderFeeReturnToCustomerService.GetApplyCount(orderIdList);

            foreach (LogisticsOrderRefundApplyInfo apply in logisticsOrderRefundApplyList)
            {
                int orderId = apply.Order?.Id ?? 0;
                if (dictApplyCount.TryGetValue(orderId, out int applyCount))
                {
                    apply.OrderApplyCountCache = applyCount;
                }
            }

            LogisticsOrderRefundApplyListModel model = new LogisticsOrderRefundApplyListModel(listPager);
            model.LogisticsOrderRefundApplyList = logisticsOrderRefundApplyList;

            return View("LogisticsOrderRefundApplyList", model);
        }

        [NonAction]
        private ICriteria GetListCriteria(LogisticsOrderRefundApplyListQueryParam queryParam)
        {
            ICriteria criteria = session.CreateCriteria<LogisticsOrderRefundApplyInfo>();

            criteria.SetReadOnly(true);

            criteria = criteria.CreateAlias(nameof(LogisticsOrderRefundApplyInfo.Order), nameof(LogisticsOrderRefundApplyInfo.Order));
            criteria = criteria.CreateAlias(nameof(LogisticsOrderRefundApplyInfo.Customer), nameof(LogisticsOrderRefundApplyInfo.Customer));
            criteria = criteria.CreateAlias(nameof(LogisticsOrderRefundApplyInfo.ApplyUser), nameof(LogisticsOrderRefundApplyInfo.ApplyUser));
            criteria = criteria.CreateAlias(nameof(LogisticsOrderRefundApplyInfo.ApproveUser), nameof(LogisticsOrderRefundApplyInfo.ApproveUser), JoinType.LeftOuterJoin);

            if (queryParam.ViewScopeQuery == ApplyDataViewScope.MySelf)
            {
                int loginUserId = loginUserService.GetLoginUserId();
                criteria = criteria.Add(Restrictions.Where<LogisticsOrderRefundApplyInfo>(e => e.ApplyUser.Id == loginUserId));
            }

            if (queryParam.TypeQuery != null && queryParam.TypeQuery != LogisticsOrderRefundType.None)
            {
                criteria = criteria.Add(Restrictions.Where<LogisticsOrderRefundApplyInfo>(e => e.Type == queryParam.TypeQuery));
            }

            if (string.IsNullOrWhiteSpace(queryParam.OrderCodeQuery) == false)
            {
                string orderCodeQuery = queryParam.OrderCodeQuery.Trim();
                criteria = criteria.Add(Restrictions.Like($"{nameof(LogisticsOrderRefundApplyInfo.Order)}.{nameof(LogisticsOrderRefundApplyInfo.Order.Code)}", orderCodeQuery, MatchMode.Anywhere));
            }

            if (string.IsNullOrWhiteSpace(queryParam.CustomerCodeQuery) == false)
            {
                int customerIdQuery = customerService.GetCustomerIdFromCustomerCode(queryParam.CustomerCodeQuery);
                criteria = criteria.Add(Restrictions.Eq($"{nameof(LogisticsOrderRefundApplyInfo.Customer)}.{nameof(LogisticsOrderRefundApplyInfo.Customer.Id)}", customerIdQuery));
            }

            string reasonQuery = queryParam.ReasonQuery;
            if (string.IsNullOrWhiteSpace(reasonQuery) == false)
            {
                reasonQuery = reasonQuery.Trim();
                criteria = criteria.Add(Restrictions.On<LogisticsOrderRefundApplyInfo>(e => e.ReasonContent).IsInsensitiveLike(reasonQuery, MatchMode.Anywhere));
            }

            if (queryParam.StatusQuery != null && queryParam.StatusQuery != LogisticsOrderRefundApplyStatus.None)
            {
                criteria = criteria.Add(Restrictions.Where<LogisticsOrderRefundApplyInfo>(e => e.Status == queryParam.StatusQuery));
            }

            return criteria;
        }

        [HttpPost]
        [Route("/admin/logistics/order/refund/apply/detail/{id:int?}")]
        [NoHibernateAutoTransaction]
        public IActionResult Detail(int? id)
        {
            session.DefaultReadOnly = true;
            LogisticsOrderRefundApplyDetailModel model = new LogisticsOrderRefundApplyDetailModel();

            if (id != null && id.Value != 0)
            {
                LogisticsOrderRefundApplyInfo logisticsOrderRefundApply = session.Get<LogisticsOrderRefundApplyInfo>(id.Value);
                if (logisticsOrderRefundApply == null)
                {
                    throw new ValidationException("指定的数据项不存在！");
                }

                model.OrderCode = logisticsOrderRefundApply.Order?.Code;
                model.CustomerCode = logisticsOrderRefundApply.Customer?.CustomerCode;
                model.Type = logisticsOrderRefundApply.Type;
                model.Amount = logisticsOrderRefundApply.Amount;
                model.ReasonId = logisticsOrderRefundApply.Reason?.Id ?? 0;
                model.ApplyMemo = logisticsOrderRefundApply.ApplyMemo;
            }

            model.ApplyId = id ?? 0;

            model.TypeList = LogisticsOrderRefundType.GetList();
            model.ReasonList = session.Query<FeeRefundReasonInfo>()
                .Where(e => e.Status == FeeRefundReasonStatus.Enabled)
                .OrderBy(e => e.Content)
                .ToList();

            return View("LogisticsOrderRefundApplyDetail", model);
        }

        [HttpPost]
        [Route("/admin/logistics/order/refund/apply/save")]
        public IActionResult Save(LogisticsOrderRefundApplyDetailModel model)
        {
            if (!ModelState.IsValid)
            {
                throw new ValidationException(ValidationUtil.GetErrorMessages(ModelState));
            }

            LogisticsOrderInfo logisticsOrder = session.Query<LogisticsOrderInfo>()
                .Where(e => e.Code == model.OrderCode)
                .Take(1)
                .SingleOrDefault();

            if (logisticsOrder == null)
            {
                throw new ValidationException("指定的订单编号不存在！");
            }

            // 客户
            int customerId = customerService.GetCustomerIdFromCustomerCode(model.CustomerCode);
            if (customerId == 0)
            {
                throw new ValidationException("客户ID输入格式有误！");
            }

            CustomerInfo customer = session.Get<CustomerInfo>(customerId);
            if (customer == null)
            {
                throw new ValidationException("指定的客户不存在！");
            }

            if (logisticsOrder.Customer.Id != customer.Id)
            {
                throw new ValidationException("指定的订单和客户不匹配！");
            }

            if (model.Amount <= 0)
            {
                throw new ValidationException("退赔款费用必须大于0！");
            }

            FeeRefundReasonInfo reason = session.Get<FeeRefundReasonInfo>(model.ReasonId);
            if (reason == null)
            {
                throw new ValidationException("指定的退赔原因不存在！");
            }

            int applyId = model.ApplyId;

            bool isCreate = applyId == 0;

            LogisticsOrderRefundApplyInfo logisticsOrderRefundApply = null;

            if (isCreate)
            {
                logisticsOrderRefundApply = new LogisticsOrderRefundApplyInfo();
                logisticsOrderRefundApply.Status = LogisticsOrderRefundApplyStatus.Saved;
            }
            else
            {
                logisticsOrderRefundApply = session.Get<LogisticsOrderRefundApplyInfo>(applyId);

                // 判断状态是不是能修改
                if (logisticsOrderRefundApply.Status != LogisticsOrderRefundApplyStatus.Saved
                    && logisticsOrderRefundApply.Status != LogisticsOrderRefundApplyStatus.Backed)
                {
                    throw new ValidationException("只有【未提交】和【已回退】状态的申请才能修改！");
                }

                int loginUserId = loginUserService.GetLoginUserId();
                if (logisticsOrderRefundApply.ApplyUser.Id != loginUserId)
                {
                    throw new ValidationException("只能修改当前登录人创建的申请！");
                }
            }

            logisticsOrderRefundApply.Order = logisticsOrder;
            logisticsOrderRefundApply.Customer = customer;
            logisticsOrderRefundApply.Type = model.Type;
            logisticsOrderRefundApply.Reason = reason;
            logisticsOrderRefundApply.ReasonContent = reason.Content ?? "";
            logisticsOrderRefundApply.Amount = model.Amount;
            logisticsOrderRefundApply.PayType = LogisticsOrderRefundPayType.Charge;
            logisticsOrderRefundApply.ApplyUser = session.Get<UserInfo>(loginUserService.GetLoginUserId());
            logisticsOrderRefundApply.ApplyMemo = model.ApplyMemo;

            if (isCreate)
            {
                session.Save(logisticsOrderRefundApply);
            }
            else
            {
                session.Update(logisticsOrderRefundApply);
            }

            session.Flush();

            return Json(new Dictionary<string, object>() {
                {"success", true},
                {"message", "保存成功！"}
            });
        }

        [HttpPost]
        [Route("/admin/logistics/order/refund/apply/submit")]
        public IActionResult Submit(int[] idList)
        {
            if (idList == null || idList.Length == 0)
            {
                throw new ValidationException("请选择要提交的数据项！");
            }

            int loginUserId = loginUserService.GetLoginUserId();

            IList<LogisticsOrderRefundApplyInfo> applyList = session
                .Query<LogisticsOrderRefundApplyInfo>()
                .Where(e => idList.Contains(e.Id))
                .ToList();

            foreach (LogisticsOrderRefundApplyInfo apply in applyList)
            {
                if (apply.ApplyUser.Id != loginUserId)
                {
                    throw new ValidationException("只能提交当前登录人保存的申请！");
                }

                if (apply.Status != LogisticsOrderRefundApplyStatus.Saved && apply.Status != LogisticsOrderRefundApplyStatus.Backed)
                {
                    throw new ValidationException("只能提交【未提交】的和【已回退】的申请！");
                }
            }

            foreach (LogisticsOrderRefundApplyInfo apply in applyList)
            {
                apply.ApplyTime = DateTime.Now;
                apply.Status = LogisticsOrderRefundApplyStatus.Submitted;
                session.Update(apply);
            }

            session.Flush();

            return Json(new Dictionary<string, object>() {
                {"success", true},
                {"message", "提交成功！"},
            });
        }

        [HttpPost]
        [Route("/admin/logistics/order/refund/apply/delete")]
        public IActionResult Delete(int[] idList)
        {
            if (idList == null || idList.Length == 0)
            {
                throw new ValidationException("请选择要删除的数据项！");
            }

            int loginUserId = loginUserService.GetLoginUserId();

            IList<LogisticsOrderRefundApplyInfo> applyList = session.Query<LogisticsOrderRefundApplyInfo>()
                .Where(e => idList.Contains(e.Id))
                .ToList();

            foreach (LogisticsOrderRefundApplyInfo apply in applyList)
            {
                if (apply.ApplyUser.Id != loginUserId)
                {
                    throw new ValidationException("只能删除当前登录人创建的申请！");
                }

                if (apply.Status != LogisticsOrderRefundApplyStatus.Saved && apply.Status != LogisticsOrderRefundApplyStatus.Backed)
                {
                    throw new ValidationException("只能删除【未提交】的和【已回退】的申请！");
                }
            }

            foreach (LogisticsOrderRefundApplyInfo apply in applyList)
            {
                session.Delete(apply);
            }

            session.Flush();

            return Json(new Dictionary<string, object>() {
                {"success", true},
                {"message", "删除成功！"},
            });
        }


        [HttpPost]
        [Route("/admin/logistics/order/refund/apply/check/order")]
        [NoHibernateAutoTransaction]
        [AdminAuthNoCheck]
        public IActionResult CheckOrder(string orderCode)
        {
            if (string.IsNullOrWhiteSpace(orderCode))
            {
                return Json(new Dictionary<string, object>() {
                    {"success", true},
                    {"message", "订单检查成功，无提示！"},
                });
            }

            LogisticsOrderInfo logisticsOrder = session.Query<LogisticsOrderInfo>()
                .Where(e => e.Code == orderCode)
                .Take(1)
                .SingleOrDefault();

            if (logisticsOrder == null)
            {
                return Json(new Dictionary<string, object>() {
                    {"success", false},
                    {"message", $"指定的订单编号 {orderCode}, 不存在！！"},
                });
            }

            int orderId = logisticsOrder?.Id ?? 0;

            IList<string> applyCountDescList = logisticsOrderFeeReturnToCustomerService.GetApplyCountDescList(orderId);

            if (applyCountDescList.Count > 0)
            {
                string message = string.Join("<br/>", applyCountDescList);

                return Json(new Dictionary<string, object>() {
                    {"success", false},
                    {"message", message},
                });
            }

            return Json(new Dictionary<string, object>() {
                {"success", true},
                {"message", "订单检查成功，无提示！"},
            });
        }
    }
}