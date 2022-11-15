using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.SqlCommand;
using shunda.Admin.Areas.Fee.Models.FeeRefundApprove;
using shunda.Admin.Services;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Customer;
using shunda.Domain.Fee;
using shunda.Domain.Logistics;
using shunda.Domain.Pass;
using shunda.Domain.Pass.PassRes;
using shunda.Service.Customer;
using shunda.Service.Fee;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Fee.Controllers;

[FeeArea]
[AdminAuth(PassRes.Fee.FeeRefundApprove)]
public class FeeRefundApproveController : Controller
{
    private ISession session;
    private CustomerCodeService customerCodeService;
    private LoginUserService loginUserService;
    private CustomerAccountService customerAccountService;
    private LogisticsOrderFeeService logisticsOrderFeeService;
    private FeeApplyCountService feeApplyCountService;

    public FeeRefundApproveController(ISession session,
        CustomerCodeService customerCodeService,
        LoginUserService loginUserService,
        CustomerAccountService customerAccountService,
        LogisticsOrderFeeService logisticsOrderFeeService,
        FeeApplyCountService feeApplyCountService)
    {
        this.session = session;
        this.customerCodeService = customerCodeService;
        this.loginUserService = loginUserService;
        this.customerAccountService = customerAccountService;
        this.logisticsOrderFeeService = logisticsOrderFeeService;
        this.feeApplyCountService = feeApplyCountService;
    }

    [HttpGet]
    [Route("/admin/logistics/order/refund/approve")]
    public IActionResult Index()
    {
        FeeRefundApproveIndexModel model = new FeeRefundApproveIndexModel();

        model.TypeList = FeeRefundType.GetList();
        model.StatusList = FeeRefundApplyStatus.GetApproveViewList();

        model.ApplyUserList = session.Query<FeeRefundApplyInfo>()
            .Fetch(e => e.ApplyUser)
            .OrderBy(e => e.ApplyUser.LoginName)
            .Select(e => e.ApplyUser)
            .Distinct()
            .ToList();

        return View("FeeRefundApproveIndex", model);
    }

    [HttpPost]
    [Route("/admin/logistics/order/refund/approve/list")]
    public IActionResult List(FeeRefundApproveListQueryParam queryParam)
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

        IDictionary<int, int> dictApplyCount = feeApplyCountService.GetApplyCount(orderIdList);

        foreach (FeeRefundApplyInfo apply in logisticsOrderRefundApplyList)
        {
            int orderId = apply.Order?.Id ?? 0;
            if (dictApplyCount.TryGetValue(orderId, out int applyCount))
            {
                apply.OrderApplyCountCache = applyCount;
            }

            apply.CustomerCodeCache = customerCodeService.GetCustomerCode(apply.Customer.Id);
        }

        FeeRefundApproveListModel model = new FeeRefundApproveListModel(listPager);
        model.FeeRefundApplyList = logisticsOrderRefundApplyList;

        return View("FeeRefundApproveList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(FeeRefundApproveListQueryParam queryParam)
    {
        ICriteria criteria = session.CreateCriteria<FeeRefundApplyInfo>();

        criteria.SetReadOnly(true);

        // criteria = criteria.CreateAlias(nameof(FeeRefundApplyInfo.Order), nameof(FeeRefundApplyInfo.Order));
        // criteria = criteria.CreateAlias(nameof(FeeRefundApplyInfo.Customer), nameof(FeeRefundApplyInfo.Customer));
        criteria = criteria.CreateAlias(nameof(FeeRefundApplyInfo.ApplyUser), nameof(FeeRefundApplyInfo.ApplyUser));
        criteria = criteria.CreateAlias(nameof(FeeRefundApplyInfo.ApproveUser), nameof(FeeRefundApplyInfo.ApproveUser), JoinType.LeftOuterJoin);

        criteria = criteria.Add(Restrictions.Where<FeeRefundApplyInfo>(e => e.Status != FeeRefundApplyStatus.Saved && e.Status != FeeRefundApplyStatus.Backed));

        if (queryParam.TypeQuery != null && queryParam.TypeQuery != FeeRefundType.None)
        {
            var typeQuery = queryParam.TypeQuery;
            criteria = criteria.Add(Restrictions.Eq(nameof(FeeRefundApplyInfo.Type), typeQuery));
        }

        if (string.IsNullOrWhiteSpace(queryParam.OrderCodeQuery) == false)
        {
            string orderCodeQuery = queryParam.OrderCodeQuery.Trim();
            criteria = criteria.Add(Restrictions.Eq($"{nameof(FeeRefundApplyInfo.Order)}.{nameof(FeeRefundApplyInfo.Order.Code)}", orderCodeQuery));
        }

        if (string.IsNullOrWhiteSpace(queryParam.CustomerCodeQuery) == false)
        {
            int customerIdQuery = customerCodeService.GetCustomerIdFromCustomerCode(queryParam.CustomerCodeQuery);
            criteria = criteria.Add(Restrictions.Eq($"{nameof(FeeRefundApplyInfo.Customer)}.{nameof(FeeRefundApplyInfo.Customer.Id)}", customerIdQuery));
        }

        if (string.IsNullOrWhiteSpace(queryParam.ReasonQuery) == false)
        {
            string reasonQuery = queryParam.ReasonQuery.Trim();
            criteria = criteria.Add(Restrictions.On<FeeRefundApplyInfo>(e => e.ReasonContent).IsInsensitiveLike(reasonQuery, MatchMode.Anywhere));
        }

        if (queryParam.StatusQuery != null && queryParam.StatusQuery != FeeRefundApplyStatus.None)
        {
            criteria = criteria.Add(Restrictions.Where<FeeRefundApplyInfo>(e => e.Status == queryParam.StatusQuery));
        }

        int applyUserIdQuery = queryParam.ApplyUserIdQuery;
        if (applyUserIdQuery != 0)
        {
            criteria = criteria.Add(Restrictions.Where<FeeRefundApplyInfo>(e => e.ApplyUser.Id == applyUserIdQuery));
        }

        return criteria;
    }

    [HttpPost]
    [Route("/admin/logistics/order/refund/approve/detail")]
    public IActionResult Detail(int id)
    {
        session.DefaultReadOnly = true;

        FeeRefundApplyInfo feeRefundApply = session.Get<FeeRefundApplyInfo>(id);
        if (feeRefundApply == null)
        {
            throw new ValidationException("指定的数据项不存在！");
        }

        feeRefundApply.CustomerCodeCache = customerCodeService.GetCustomerCode(feeRefundApply.Customer.Id);

        FeeRefundApproveDetailModel model = new FeeRefundApproveDetailModel();
        model.FeeRefundApply = feeRefundApply;
        model.ApplyId = feeRefundApply.Id;
        model.ApproveMemo = feeRefundApply.ApproveMemo;

        return View("FeeRefundApproveDetail", model);
    }

    [HttpPost]
    [Route("/admin/logistics/order/refund/approve/approve")]
    public IActionResult Approve(FeeRefundApproveDetailModel model)
    {
        if (!ModelState.IsValid)
        {
            throw new ValidationException(ValidationUtil.GetErrorMessages(ModelState));
        }

        int applyId = model.ApplyId;
        string approveMemo = model.ApproveMemo;

        FeeRefundApplyInfo refundApply = session.Get<FeeRefundApplyInfo>(applyId);
        if (refundApply == null)
        {
            throw new ValidationException("指定的申请单不存在！");
        }

        if (refundApply.Status != FeeRefundApplyStatus.Submitted)
        {
            throw new ValidationException("当前申请单不是 已提交 状态，不能审核！");
        }

        refundApply.ApproveMemo = approveMemo;
        refundApply.ApproveTime = DateTime.Now;
        refundApply.ApproveUser = session.Load<UserInfo>(loginUserService.GetLoginUserId());
        refundApply.Status = FeeRefundApplyStatus.Approved;

        session.Update(refundApply);
        session.Flush();

        //  生成退赔款记录

        FeeRefundInfo refund = new FeeRefundInfo();
        refund.Customer = refundApply.Customer;
        refund.Order = refundApply.Order;
        refund.Type = refundApply.Type;
        refund.Reason = refundApply.Reason;
        refund.ReasonContent = refundApply.ReasonContent;
        refund.Amount = refundApply.Amount;
        refund.PayType = refundApply.PayType;
        refund.Status = FeeRefundStatus.Paid;
        refund.Apply = refundApply;

        session.Save(refund);
        session.Flush();

        if (refund.Type == FeeRefundType.Refund) // 如果是【退】款的，需要加一条抵充；如果是【赔】款的，就不需要加上一条抵充了。
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

        // 余额充值
        customerAccountService.Change(CustomerAccountChangeType.Refund, refundApply.Customer.Id, refundApply.Amount, $"订单 {refundApply.Order.Code} 退赔款");

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "保存成功！" }
        });
    }

    [HttpPost]
    [Route("/admin/logistics/order/refund/approve/back")]
    [Transactional]
    public IActionResult Back(FeeRefundApproveDetailModel model)
    {
        if (!ModelState.IsValid)
        {
            throw new ValidationException(ValidationUtil.GetErrorMessages(ModelState));
        }

        int applyId = model.ApplyId;
        string approveMemo = model.ApproveMemo;

        FeeRefundApplyInfo refundApply = session.Get<FeeRefundApplyInfo>(applyId);
        if (refundApply == null)
        {
            throw new ValidationException("指定的申请单不存在！");
        }

        if (refundApply.Status != FeeRefundApplyStatus.Submitted)
        {
            throw new ValidationException("当前申请单不是 已提交 状态，不能回退 ！");
        }

        refundApply.ApproveMemo = approveMemo;
        refundApply.Status = FeeRefundApplyStatus.Backed;

        session.Update(refundApply);
        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "保存成功！" }
        });
    }
}