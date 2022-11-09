using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Util;
using Domain.Customer;
using Domain.Logistics;
using Domain.LogisticsOrder;
using Domain.Pass;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.SqlCommand;
using Service;
using Service.Customer;
using Service.LogisticsFee;
using Service.LogisticsOrder;
using Web.Areas.Admin.Models.LogisticsOrderRefundApprove;

namespace Web.Areas.Admin.Controllers
{
    [AdminArea]
    [AdminAuth(PassRes.OrderFee.RefundApprove)]
    public class LogisticsOrderRefundApproveController : Controller
    {
        private ISession session = null;
        private CustomerService customerService = null;
        private LoginUserService loginUserService = null;
        private CustomerAccountService customerAccountService = null;
        private LogisticsOrderFeeService logisticsOrderFeeService = null;
        private LogisticsOrderFeeReturnToCustomerService logisticsOrderFeeReturnToCustomerService;

        public LogisticsOrderRefundApproveController(ISession session,
            CustomerService customerService,
            LoginUserService loginUserService,
            CustomerAccountService customerAccountService,
            LogisticsOrderFeeService logisticsOrderFeeService,
            LogisticsOrderFeeReturnToCustomerService logisticsOrderFeeReturnToCustomerService)
        {
            this.session = session;
            this.customerService = customerService;
            this.loginUserService = loginUserService;
            this.customerAccountService = customerAccountService;
            this.logisticsOrderFeeService = logisticsOrderFeeService;
            this.logisticsOrderFeeReturnToCustomerService = logisticsOrderFeeReturnToCustomerService;
        }

        [HttpGet]
        [Route("/admin/logistics/order/refund/approve")]
        [NoHibernateAutoTransaction]
        public IActionResult Index()
        {
            LogisticsOrderRefundApproveIndexModel model = new LogisticsOrderRefundApproveIndexModel();

            model.TypeList = LogisticsOrderRefundType.GetList();
            model.StatusList = LogisticsOrderRefundApplyStatus.GetApproveViewList();

            model.ApplyUserList = session.Query<LogisticsOrderRefundApplyInfo>()
                .Fetch(e => e.ApplyUser)
                .OrderBy(e => e.ApplyUser.LoginName)
                .Select(e => e.ApplyUser)
                .Distinct()
                .ToList();

            return View("LogisticsOrderRefundApproveIndex", model);
        }

        [HttpPost]
        [Route("/admin/logistics/order/refund/approve/list")]
        [NoHibernateAutoTransaction]
        public IActionResult List(LogisticsOrderRefundApproveListQueryParam queryParam)
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

            LogisticsOrderRefundApproveListModel model = new LogisticsOrderRefundApproveListModel(listPager);
            model.LogisticsOrderRefundApplyList = logisticsOrderRefundApplyList;

            return View("LogisticsOrderRefundApproveList", model);
        }

        [NonAction]
        private ICriteria GetListCriteria(LogisticsOrderRefundApproveListQueryParam queryParam)
        {
            ICriteria criteria = session.CreateCriteria<LogisticsOrderRefundApplyInfo>();

            criteria.SetReadOnly(true);

            criteria = criteria.CreateAlias(nameof(LogisticsOrderRefundApplyInfo.Order), nameof(LogisticsOrderRefundApplyInfo.Order));
            criteria = criteria.CreateAlias(nameof(LogisticsOrderRefundApplyInfo.Customer), nameof(LogisticsOrderRefundApplyInfo.Customer));
            criteria = criteria.CreateAlias(nameof(LogisticsOrderRefundApplyInfo.ApplyUser), nameof(LogisticsOrderRefundApplyInfo.ApplyUser));
            criteria = criteria.CreateAlias(nameof(LogisticsOrderRefundApplyInfo.ApproveUser), nameof(LogisticsOrderRefundApplyInfo.ApproveUser), JoinType.LeftOuterJoin);

            criteria = criteria.Add(Restrictions.Where<LogisticsOrderRefundApplyInfo>(e => e.Status != LogisticsOrderRefundApplyStatus.Saved && e.Status != LogisticsOrderRefundApplyStatus.Backed));

            if (queryParam.TypeQuery != null && queryParam.TypeQuery != LogisticsOrderRefundType.None)
            {
                var typeQuery = queryParam.TypeQuery;
                criteria = criteria.Add(Restrictions.Eq(nameof(LogisticsOrderRefundApplyInfo.Type), typeQuery));
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

            int applyUserIdQuery = queryParam.ApplyUserIdQuery;
            if (applyUserIdQuery != 0)
            {
                criteria = criteria.Add(Restrictions.Where<LogisticsOrderRefundApplyInfo>(e => e.ApplyUser.Id == applyUserIdQuery));
            }

            return criteria;
        }

        [HttpPost]
        [Route("/admin/logistics/order/refund/approve/detail")]
        [NoHibernateAutoTransaction]
        public IActionResult Detail(int id)
        {
            session.DefaultReadOnly = true;

            LogisticsOrderRefundApplyInfo logisticsOrderRefundApply = session.Get<LogisticsOrderRefundApplyInfo>(id);
            if (logisticsOrderRefundApply == null)
            {
                throw new ValidationException("指定的数据项不存在！");
            }

            LogisticsOrderRefundApproveDetailModel model = new LogisticsOrderRefundApproveDetailModel();
            model.LogisticsOrderRefundApply = logisticsOrderRefundApply;
            model.ApplyId = logisticsOrderRefundApply.Id;
            model.ApproveMemo = logisticsOrderRefundApply.ApproveMemo;

            return View("LogisticsOrderRefundApproveDetail", model);
        }

        [HttpPost]
        [Route("/admin/logistics/order/refund/approve/approve")]
        public IActionResult Approve(LogisticsOrderRefundApproveDetailModel model)
        {
            if (!ModelState.IsValid)
            {
                throw new ValidationException(ValidationUtil.GetErrorMessages(ModelState));
            }

            int applyId = model.ApplyId;
            string approveMemo = model.ApproveMemo;

            LogisticsOrderRefundApplyInfo refundApply = session.Get<LogisticsOrderRefundApplyInfo>(applyId);
            if (refundApply == null)
            {
                throw new ValidationException("指定的申请单不存在！");
            }

            if (refundApply.Status != LogisticsOrderRefundApplyStatus.Submitted)
            {
                throw new ValidationException("当前申请单不是 已提交 状态，不能审核！");
            }

            refundApply.ApproveMemo = approveMemo;
            refundApply.ApproveTime = DateTime.Now;
            refundApply.ApproveUser = session.Load<UserInfo>(loginUserService.GetLoginUserId());
            refundApply.Status = LogisticsOrderRefundApplyStatus.Approved;

            session.Update(refundApply);
            session.Flush();

            //  生成退赔款记录

            LogisticsOrderRefundInfo refund = new LogisticsOrderRefundInfo();
            refund.Customer = refundApply.Customer;
            refund.Order = refundApply.Order;
            refund.Type = refundApply.Type;
            refund.Reason = refundApply.Reason;
            refund.ReasonContent = refundApply.ReasonContent;
            refund.Amount = refundApply.Amount;
            refund.PayType = refundApply.PayType;
            refund.Status = LogisticsOrderRefundStatus.Paid;
            refund.Apply = refundApply;

            session.Save(refund);
            session.Flush();

            if (refund.Type == LogisticsOrderRefundType.Refund) // 如果是【退】款的，需要加一条抵充；如果是【赔】款的，就不需要加上一条抵充了。
            {
                // 记录订单费用
                // 如果是退款的话，一般是客户多付了，退给他们后，需要增加一条费用平衡原来多付的。
                LogisticsOrderFeeInfo logisticsOrderFeeInfo = new LogisticsOrderFeeInfo();
                logisticsOrderFeeInfo.Customer = refundApply.Customer; // 一般情况下，这个 会员 和 订单 是匹配的
                logisticsOrderFeeInfo.Order = refundApply.Order;
                logisticsOrderFeeInfo.GoodsType = LogisticsGoodsType.None;
                logisticsOrderFeeInfo.Type = LogisticsOrderFeeType.RefundFee;
                logisticsOrderFeeInfo.Fee = refundApply.Amount;
                logisticsOrderFeeInfo.Description = refundApply.ReasonContent;

                session.Save(logisticsOrderFeeInfo);
                session.Flush();
            }

            // 费用变动后，这个也要调用一下的 
            logisticsOrderFeeService.SumFeeAndUpdatePayStatus(refundApply.Order);

            var orderCate = refundApply.Order?.Cate ?? LogisticsOrderCate.Sea;
            // 余额充值
            customerAccountService.Change(orderCate, CustomerAccountChangeType.Refund, refundApply.Customer.Id, refundApply.Amount, $"订单 {refundApply.Order.Code} 退赔款");

            return Json(new Dictionary<string, object>() {
                { "success", true },
                { "message", "保存成功！" }
            });
        }

        [HttpPost]
        [Route("/admin/logistics/order/refund/approve/back")]
        public IActionResult Back(LogisticsOrderRefundApproveDetailModel model)
        {
            if (!ModelState.IsValid)
            {
                throw new ValidationException(ValidationUtil.GetErrorMessages(ModelState));
            }

            int applyId = model.ApplyId;
            string approveMemo = model.ApproveMemo;

            LogisticsOrderRefundApplyInfo refundApply = session.Get<LogisticsOrderRefundApplyInfo>(applyId);
            if (refundApply == null)
            {
                throw new ValidationException("指定的申请单不存在！");
            }

            if (refundApply.Status != LogisticsOrderRefundApplyStatus.Submitted)
            {
                throw new ValidationException("当前申请单不是 已提交 状态，不能回退 ！");
            }

            refundApply.ApproveMemo = approveMemo;
            refundApply.Status = LogisticsOrderRefundApplyStatus.Backed;

            session.Update(refundApply);
            session.Flush();

            return Json(new Dictionary<string, object>() {
                { "success", true },
                { "message", "保存成功！" }
            });
        }
    }
}