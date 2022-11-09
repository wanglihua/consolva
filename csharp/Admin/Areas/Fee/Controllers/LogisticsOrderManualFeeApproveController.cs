using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NLog;
using shunda.Admin.Areas.Fee.Models.ManualFeeApprove.Web.Areas.Admin.Models.ManualFeeApprove;
using shunda.Admin.Services;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Web;
using shunda.Domain.Fee;
using shunda.Domain.Order;
using shunda.Domain.Pass;
using shunda.Domain.Pass.PassRes;
using shunda.Service.Fee;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Fee.Controllers;

[FeeArea]
[AdminAuth(PassRes.Fee.ManualDecreaseFeeApprove)]
public class ManualFeeApproveController : Controller
{
    private static Logger log = LogManager.GetCurrentClassLogger();

    private ISession session;
    private CustomerService customerService;
    private LoginUserService loginUserService;
    private ManualFeeService manualFeeService;
    private LogisticsOrderFeeReturnToCustomerService logisticsOrderFeeReturnToCustomerService;
    private LogisticsOrderService logisticsOrderService;

    public ManualFeeApproveController(ISession session,
        CustomerService customerService,
        LoginUserService loginUserService,
        ManualFeeService manualFeeService,
        LogisticsOrderFeeReturnToCustomerService logisticsOrderFeeReturnToCustomerService,
        LogisticsOrderService logisticsOrderService)
    {
        this.session = session;
        this.customerService = customerService;
        this.loginUserService = loginUserService;
        this.manualFeeService = manualFeeService;
        this.logisticsOrderFeeReturnToCustomerService = logisticsOrderFeeReturnToCustomerService;
        this.logisticsOrderService = logisticsOrderService;
    }

    [HttpGet]
    [Route("/admin/manual/fee/approve")]
    public IActionResult Index()
    {
        ManualFeeApproveIndexModel model = new ManualFeeApproveIndexModel();

        model.FeeReasonList = session.Query<ManualFeeReasonInfo>().Where(e => e.FeeType == ManualFeeType.ManualDecrease).ToList();
        model.ApplyStatusList = ManualFeeApplyStatus.GetApproveViewList();

        model.ApplyUserList = session.Query<ManualFeeApplyInfo>()
            .Fetch(e => e.ApplyUser)
            .OrderBy(e => e.ApplyUser.LoginName)
            .Select(e => e.ApplyUser)
            .Distinct()
            .ToList();

        return View("ManualFeeApproveIndex", model);
    }

    [HttpPost]
    [Route("/admin/manual/fee/approve/list")]
    public IActionResult List(ManualFeeApproveListQueryParam queryParam)
    {
        int totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<ManualFeeApplyInfo> logisticsOrderManualFeeApplyList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<ManualFeeApplyInfo>();

        IList<int> orderIdList = logisticsOrderManualFeeApplyList.Select(e => (e.Order?.Id ?? 0)).ToList();

        /*
        IList<ManualFeeApplyCountInfo> applyCountList = session.Query<ManualFeeApplyCountInfo>()
            .Where(e => orderIdList.Contains(e.OrderId))
            .ToList();

        foreach (ManualFeeApplyInfo apply in logisticsOrderManualFeeApplyList)
        {
            int orderId = apply.Order?.Id ?? 0;
            apply.OrderApplyCountCache = applyCountList.Where(e => e.OrderId == orderId).Take(1).SingleOrDefault()?.ApplyCount ?? 0;
        }
        */

        IDictionary<int, int> dictApplyCount = logisticsOrderFeeReturnToCustomerService.GetApplyCount(orderIdList);

        foreach (ManualFeeApplyInfo apply in logisticsOrderManualFeeApplyList)
        {
            int orderId = apply.Order?.Id ?? 0;
            if (dictApplyCount.TryGetValue(orderId, out int applyCount))
            {
                apply.OrderApplyCountCache = applyCount;
            }
        }

        ManualFeeApproveListModel model = new ManualFeeApproveListModel(listPager);
        model.ManualFeeApplyList = logisticsOrderManualFeeApplyList;

        return View("ManualFeeApproveList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(ManualFeeApproveListQueryParam queryParam)
    {
        ICriteria criteria = session.CreateCriteria<ManualFeeApplyInfo>();

        criteria = criteria.CreateAlias(nameof(ManualFeeApplyInfo.Order), nameof(ManualFeeApplyInfo.Order));
        criteria = criteria.CreateAlias(nameof(ManualFeeApplyInfo.Customer), nameof(ManualFeeApplyInfo.Customer));
        criteria = criteria.CreateAlias(nameof(ManualFeeApplyInfo.ApplyUser), nameof(ManualFeeApplyInfo.ApplyUser));
        criteria = criteria.CreateAlias(nameof(ManualFeeApplyInfo.ApproveUser), nameof(ManualFeeApplyInfo.ApproveUser), JoinType.LeftOuterJoin);

        // 只显示【已提交】和【已审核】的申请
        criteria = criteria.Add(Restrictions.Where<ManualFeeApplyInfo>(e => e.Status == ManualFeeApplyStatus.Submitted || e.Status == ManualFeeApplyStatus.Approved));

        criteria.SetReadOnly(true);

        if (queryParam.ManualReasonIdQuery != 0)
        {
            criteria = criteria.Add(Restrictions.Where<ManualFeeApplyInfo>(e => e.ManualReason.Id == queryParam.ManualReasonIdQuery));
        }

        if (string.IsNullOrWhiteSpace(queryParam.OrderCodeQuery) == false)
        {
            string orderCodeQuery = queryParam.OrderCodeQuery.Trim();
            criteria = criteria.Add(Restrictions.Like($"{nameof(ManualFeeApplyInfo.Order)}.{nameof(ManualFeeApplyInfo.Order.Code)}", orderCodeQuery, MatchMode.Anywhere));
        }

        if (string.IsNullOrWhiteSpace(queryParam.CustomerCodeQuery) == false)
        {
            int customerIdQuery = customerService.GetCustomerIdFromCustomerCode(queryParam.CustomerCodeQuery);
            criteria = criteria.Add(Restrictions.Eq($"{nameof(ManualFeeApplyInfo.Customer)}.{nameof(ManualFeeApplyInfo.Customer.Id)}", customerIdQuery));
        }

        if (queryParam.StatusQuery != null && queryParam.StatusQuery != ManualFeeApplyStatus.None)
        {
            criteria = criteria.Add(Restrictions.Where<ManualFeeApplyInfo>(e => e.Status == queryParam.StatusQuery));
        }

        int applyUserIdQuery = queryParam.ApplyUserIdQuery;
        if (applyUserIdQuery != 0)
        {
            criteria = criteria.Add(Restrictions.Where<ManualFeeApplyInfo>(e => e.ApplyUser.Id == applyUserIdQuery));
        }

        return criteria;
    }

    [HttpPost]
    [Route("/admin/manual/fee/approve/detail/{id:int?}")]
    public IActionResult Detail(int? id)
    {
        session.DefaultReadOnly = true;

        ManualFeeApplyInfo logisticsOrderManualFeeApply = null;

        if (id == null || id.Value == 0)
        {
            logisticsOrderManualFeeApply = new ManualFeeApplyInfo();
        }
        else
        {
            logisticsOrderManualFeeApply = session.Get<ManualFeeApplyInfo>(id.Value);
            if (logisticsOrderManualFeeApply == null)
            {
                throw new ValidationException("指定的数据项不存在！");
            }
        }

        ManualFeeApproveDetailModel model = new ManualFeeApproveDetailModel();
        model.ManualFeeApply = logisticsOrderManualFeeApply;

        return View("ManualFeeApproveDetail", model);
    }

    [HttpPost]
    [Route("/admin/manual/fee/approve/back")]
    [Transactional]
    public IActionResult Back(int[] idList)
    {
        if (idList == null || idList.Length == 0)
        {
            throw new ValidationException("请选择要回退的数据项！");
        }

        IList<ManualFeeApplyInfo> logisticsOrderManualFeeApplyList = session
            .Query<ManualFeeApplyInfo>()
            .Where(e => idList.Contains(e.Id))
            .ToList();

        foreach (ManualFeeApplyInfo logisticsOrderManualFeeApply in logisticsOrderManualFeeApplyList)
        {
            if (logisticsOrderManualFeeApply.Status != ManualFeeApplyStatus.Submitted)
            {
                throw new ValidationException("只能回退【已提交】的申请！");
            }
        }

        foreach (ManualFeeApplyInfo logisticsOrderManualFeeApply in logisticsOrderManualFeeApplyList)
        {
            logisticsOrderManualFeeApply.Status = ManualFeeApplyStatus.Backed;
            session.Update(logisticsOrderManualFeeApply);
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "回退成功！" },
        });
    }

    [HttpPost]
    [Route("/admin/manual/fee/approve/approve")]
    [Transactional]
    public IActionResult Approve(int[] idList)
    {
        if (idList == null || idList.Length == 0)
        {
            throw new ValidationException("请选择要审核的数据项！");
        }

        IList<ManualFeeApplyInfo> applyList = null;

        ITransaction transaction = null;
        try
        {
            transaction = session.BeginTransaction();

            applyList = session
                .Query<ManualFeeApplyInfo>()
                .Fetch(e => e.Order)
                .Fetch(e => e.Customer)
                .Where(e => idList.Contains(e.Id))
                .ToList();

            foreach (ManualFeeApplyInfo apply in applyList)
            {
                if (apply.Status != ManualFeeApplyStatus.Submitted)
                {
                    throw new ValidationException("只能审核【已提交】的申请！");
                }
            }

            foreach (ManualFeeApplyInfo apply in applyList)
            {
                if (apply.Type == LogisticsOrderFeeType.ManualIncrease)
                {
                    manualFeeService.AddManualIncreaseFee(
                        apply.Order,
                        apply.Customer,
                        apply.Fee,
                        apply.ManualReasonId,
                        apply.Description);
                }

                if (apply.Type == LogisticsOrderFeeType.ManualDecrease)
                {
                    manualFeeService.AddManualDecreaseFee(
                        apply.Order,
                        apply.Customer,
                        apply.Fee,
                        apply.ManualReasonId,
                        apply.Description);
                }
            }

            foreach (ManualFeeApplyInfo apply in applyList)
            {
                apply.Status = ManualFeeApplyStatus.Approved;
                apply.ApproveUser = session.Get<UserInfo>(loginUserService.GetLoginUserId());
                session.Update(apply);
            }

            session.Flush();

            transaction.Commit();
        }
        catch (Exception exp)
        {
            log.Error(exp);
            transaction?.Rollback();

            throw; // 重新抛出异常
        }

        // 支付成功后提交打包

        if (applyList != null)
        {
            foreach (ManualFeeApplyInfo apply in applyList)
            {
                try
                {
                    transaction = session.BeginTransaction();

                    LogisticsOrderInfo order = apply.Order;

                    if (order.Cate == LogisticsOrderCate.Sea
                        && order.OrderStatus == LogisticsOrderStatus.CnAllCheckin
                        && order.FeeMethod == LogisticsOrderFeeMethod.Express
                        && order.ArrearsAmountNative <= 0)
                    {
                        logisticsOrderService.SubmitPackWhenPaid(order?.Id ?? 0);
                    }

                    transaction.Commit();
                }
                catch (Exception exp)
                {
                    transaction?.Rollback();

                    log.Error(exp);

                    // 这里不重新抛出异常
                }
            }
        }

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "审核成功！" },
        });
    }
}