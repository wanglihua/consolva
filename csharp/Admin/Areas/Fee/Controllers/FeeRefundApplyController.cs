using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using shunda.Admin.Services;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Customer;
using shunda.Domain.Fee;
using shunda.Domain.Order;
using shunda.Domain.Pass;
using shunda.Domain.Pass.PassRes;
using shunda.Service.Customer;
using Web.Areas.Admin.Models.FeeRefundApply;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Fee.Controllers;

[FeeArea]
[AdminAuth(PassRes.Fee.FeeRefundApply)]
public class FeeRefundApplyController : Controller
{
    private ISession session = null;
    private CustomerCodeService customerCodeService = null;
    private LoginUserService loginUserService = null;
    private LogisticsOrderFeeReturnToCustomerService logisticsOrderFeeReturnToCustomerService;

    public FeeRefundApplyController(
        ISession session,
        CustomerCodeService customerCodeService,
        LoginUserService loginUserService,
        LogisticsOrderFeeReturnToCustomerService logisticsOrderFeeReturnToCustomerService)
    {
        this.session = session;
        this.customerCodeService = customerCodeService;
        this.loginUserService = loginUserService;
        this.logisticsOrderFeeReturnToCustomerService = logisticsOrderFeeReturnToCustomerService;
    }

    [HttpGet]
    [Route("/admin/fee/refund/apply")]
    public IActionResult Index()
    {
        FeeRefundApplyIndexModel model = new FeeRefundApplyIndexModel();

        model.TypeList = FeeRefundType.GetList();
        model.StatusList = FeeRefundApplyStatus.GetList();
        model.ScopeList = FeeApplyDataViewScope.GetList();

        return View("FeeRefundApplyIndex", model);
    }

    [HttpPost]
    [Route("/admin/fee/refund/apply/list")]
    public IActionResult List(FeeRefundApplyListQueryParam queryParam)
    {
        int totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<FeeRefundApplyInfo> logisticsOrderRefundApplyList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<FeeRefundApplyInfo>();

        IList<int> orderIdList = logisticsOrderRefundApplyList.Select(e => e.Order?.Id ?? 0).ToList();

        /*
        IList<FeeRefundApplyCountInfo> applyCountList = session
            .Query<FeeRefundApplyCountInfo>()
            .Where(e => orderIdList.Contains(e.OrderId))
            .ToList();

        foreach (FeeRefundApplyInfo apply in logisticsOrderRefundApplyList)
        {
            int orderId = apply.Order?.Id ?? 00;
            apply.OrderApplyCountCache = applyCountList.Where(e => e.OrderId == orderId).Take(1).SingleOrDefault()?.ApplyCount ?? 0;
        }
        */

        IDictionary<int, int> dictApplyCount = logisticsOrderFeeReturnToCustomerService.GetApplyCount(orderIdList);

        foreach (FeeRefundApplyInfo apply in logisticsOrderRefundApplyList)
        {
            int orderId = apply.Order?.Id ?? 0;
            if (dictApplyCount.TryGetValue(orderId, out int applyCount))
            {
                apply.OrderApplyCountCache = applyCount;
            }
        }

        FeeRefundApplyListModel model = new FeeRefundApplyListModel(listPager);
        model.FeeRefundApplyList = logisticsOrderRefundApplyList;

        return View("FeeRefundApplyList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(FeeRefundApplyListQueryParam queryParam)
    {
        ICriteria criteria = session.CreateCriteria<FeeRefundApplyInfo>();

        criteria.SetReadOnly(true);

        criteria = criteria.CreateAlias(nameof(FeeRefundApplyInfo.Order), nameof(FeeRefundApplyInfo.Order));
        criteria = criteria.CreateAlias(nameof(FeeRefundApplyInfo.Customer), nameof(FeeRefundApplyInfo.Customer));
        criteria = criteria.CreateAlias(nameof(FeeRefundApplyInfo.ApplyUser), nameof(FeeRefundApplyInfo.ApplyUser));
        criteria = criteria.CreateAlias(nameof(FeeRefundApplyInfo.ApproveUser), nameof(FeeRefundApplyInfo.ApproveUser), JoinType.LeftOuterJoin);

        if (queryParam.ViewScopeQuery == FeeApplyDataViewScope.MySelf)
        {
            int loginUserId = loginUserService.GetLoginUserId();
            criteria = criteria.Add(Restrictions.Where<FeeRefundApplyInfo>(e => e.ApplyUser.Id == loginUserId));
        }

        if (queryParam.TypeQuery != null && queryParam.TypeQuery != FeeRefundType.None)
        {
            criteria = criteria.Add(Restrictions.Where<FeeRefundApplyInfo>(e => e.Type == queryParam.TypeQuery));
        }

        if (string.IsNullOrWhiteSpace(queryParam.OrderCodeQuery) == false)
        {
            string orderCodeQuery = queryParam.OrderCodeQuery.Trim();
            criteria = criteria.Add(Restrictions.Like($"{nameof(FeeRefundApplyInfo.Order)}.{nameof(FeeRefundApplyInfo.Order.Code)}", orderCodeQuery, MatchMode.Anywhere));
        }

        if (string.IsNullOrWhiteSpace(queryParam.CustomerCodeQuery) == false)
        {
            int customerIdQuery = customerCodeService.GetCustomerIdFromCustomerCode(queryParam.CustomerCodeQuery);
            criteria = criteria.Add(Restrictions.Eq($"{nameof(FeeRefundApplyInfo.Customer)}.{nameof(FeeRefundApplyInfo.Customer.Id)}", customerIdQuery));
        }

        string reasonQuery = queryParam.ReasonQuery;
        if (string.IsNullOrWhiteSpace(reasonQuery) == false)
        {
            reasonQuery = reasonQuery.Trim();
            criteria = criteria.Add(Restrictions.On<FeeRefundApplyInfo>(e => e.ReasonContent).IsInsensitiveLike(reasonQuery, MatchMode.Anywhere));
        }

        if (queryParam.StatusQuery != null && queryParam.StatusQuery != FeeRefundApplyStatus.None)
        {
            criteria = criteria.Add(Restrictions.Where<FeeRefundApplyInfo>(e => e.Status == queryParam.StatusQuery));
        }

        return criteria;
    }

    [HttpPost]
    [Route("/admin/fee/refund/apply/detail/{id:int?}")]
    public IActionResult Detail(int? id)
    {
        session.DefaultReadOnly = true;
        FeeRefundApplyDetailModel model = new FeeRefundApplyDetailModel();

        if (id != null && id.Value != 0)
        {
            FeeRefundApplyInfo logisticsOrderRefundApply = session.Get<FeeRefundApplyInfo>(id.Value);
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

        model.TypeList = FeeRefundType.GetList();
        model.ReasonList = session.Query<FeeRefundReasonInfo>()
            .Where(e => e.Status == FeeRefundReasonStatus.Enabled)
            .OrderBy(e => e.Content)
            .ToList();

        return View("FeeRefundApplyDetail", model);
    }

    [HttpPost]
    [Route("/admin/fee/refund/apply/save")]
    [Transactional]
    public IActionResult Save(FeeRefundApplySaveModel model)
    {
        if (!ModelState.IsValid)
        {
            throw new ValidationException(ValidationUtil.GetErrorMessages(ModelState));
        }

        LogisticsOrderInfo? logisticsOrder = session.Query<LogisticsOrderInfo>()
            .Where(e => e.Code == model.OrderCode)
            .Take(1)
            .SingleOrDefault();

        if (logisticsOrder == null)
        {
            throw new ValidationException("指定的订单编号不存在！");
        }

        // 客户
        int customerId = customerCodeService.GetCustomerIdFromCustomerCode(model.CustomerCode);
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

        FeeRefundApplyInfo logisticsOrderRefundApply = null;

        if (isCreate)
        {
            logisticsOrderRefundApply = new FeeRefundApplyInfo();
            logisticsOrderRefundApply.Status = FeeRefundApplyStatus.Saved;
        }
        else
        {
            logisticsOrderRefundApply = session.Get<FeeRefundApplyInfo>(applyId);

            // 判断状态是不是能修改
            if (logisticsOrderRefundApply.Status != FeeRefundApplyStatus.Saved
                && logisticsOrderRefundApply.Status != FeeRefundApplyStatus.Backed)
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
        logisticsOrderRefundApply.PayType = FeeRefundPayType.Charge;
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
            { "success", true },
            { "message", "保存成功！" }
        });
    }

    [HttpPost]
    [Route("/admin/fee/refund/apply/submit")]
    [Transactional]
    public IActionResult Submit(int[] idList)
    {
        if (idList == null || idList.Length == 0)
        {
            throw new ValidationException("请选择要提交的数据项！");
        }

        int loginUserId = loginUserService.GetLoginUserId();

        IList<FeeRefundApplyInfo> applyList = session
            .Query<FeeRefundApplyInfo>()
            .Where(e => idList.Contains(e.Id))
            .ToList();

        foreach (FeeRefundApplyInfo apply in applyList)
        {
            if (apply.ApplyUser.Id != loginUserId)
            {
                throw new ValidationException("只能提交当前登录人保存的申请！");
            }

            if (apply.Status != FeeRefundApplyStatus.Saved && apply.Status != FeeRefundApplyStatus.Backed)
            {
                throw new ValidationException("只能提交【未提交】的和【已回退】的申请！");
            }
        }

        foreach (FeeRefundApplyInfo apply in applyList)
        {
            apply.ApplyTime = DateTime.Now;
            apply.Status = FeeRefundApplyStatus.Submitted;
            session.Update(apply);
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "提交成功！" },
        });
    }

    [HttpPost]
    [Route("/admin/fee/refund/apply/delete")]
    [Transactional]
    public IActionResult Delete(int[] idList)
    {
        if (idList == null || idList.Length == 0)
        {
            throw new ValidationException("请选择要删除的数据项！");
        }

        int loginUserId = loginUserService.GetLoginUserId();

        IList<FeeRefundApplyInfo> applyList = session.Query<FeeRefundApplyInfo>()
            .Where(e => idList.Contains(e.Id))
            .ToList();

        foreach (FeeRefundApplyInfo apply in applyList)
        {
            if (apply.ApplyUser.Id != loginUserId)
            {
                throw new ValidationException("只能删除当前登录人创建的申请！");
            }

            if (apply.Status != FeeRefundApplyStatus.Saved && apply.Status != FeeRefundApplyStatus.Backed)
            {
                throw new ValidationException("只能删除【未提交】的和【已回退】的申请！");
            }
        }

        foreach (FeeRefundApplyInfo apply in applyList)
        {
            session.Delete(apply);
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "删除成功！" },
        });
    }


    [HttpPost]
    [Route("/admin/fee/refund/apply/check/order")]
    [NoLoginRequired]
    public IActionResult CheckOrder(string orderCode)
    {
        if (string.IsNullOrWhiteSpace(orderCode))
        {
            return Json(new Dictionary<string, object>() {
                { "success", true },
                { "message", "订单检查成功，无提示！" },
            });
        }

        LogisticsOrderInfo? logisticsOrder = session.Query<LogisticsOrderInfo>()
            .Where(e => e.Code == orderCode)
            .Take(1)
            .SingleOrDefault();

        if (logisticsOrder == null)
        {
            return Json(new Dictionary<string, object>() {
                { "success", false },
                { "message", $"指定的订单编号 {orderCode}, 不存在！！" },
            });
        }

        int orderId = logisticsOrder?.Id ?? 0;

        IList<string> applyCountDescList = logisticsOrderFeeReturnToCustomerService.GetApplyCountDescList(orderId);

        if (applyCountDescList.Count > 0)
        {
            string message = string.Join("<br/>", applyCountDescList);

            return Json(new Dictionary<string, object>() {
                { "success", false },
                { "message", message },
            });
        }

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "订单检查成功，无提示！" },
        });
    }
}