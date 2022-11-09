using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using NLog;
using shunda.Admin.Areas.Fee.Models.LogisticsOrderFeeWriteOffApprove.Web.Areas.Admin.Models.LogisticsOrderFeeWriteOffApprove;
using shunda.Admin.Services;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Web;
using shunda.Domain.Fee;
using shunda.Domain.Order;
using shunda.Domain.Pass;
using shunda.Domain.Pass.PassRes;
using shunda.Service.Customer;
using shunda.Service.Fee;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Fee.Controllers;

[FeeArea]
[AdminAuth(PassRes.Fee.WriteOffFeeApprove)]
public class LogisticsOrderFeeWriteOffApproveController : Controller
{
    private static Logger log = LogManager.GetCurrentClassLogger();

    private ISession session = null;
    private CustomerCodeService customerCodeService = null;
    private LoginUserService loginUserService = null;
    private WriteOffFeeService writeOffFeeService = null;
    private LogisticsOrderFeeReturnToCustomerService logisticsOrderFeeReturnToCustomerService = null;
    private LogisticsOrderService logisticsOrderService = null;

    public LogisticsOrderFeeWriteOffApproveController(ISession session,
        CustomerCodeService customerCodeService,
        LoginUserService loginUserService,
        WriteOffFeeService writeOffFeeService,
        LogisticsOrderFeeReturnToCustomerService logisticsOrderFeeReturnToCustomerService,
        LogisticsOrderService logisticsOrderService)
    {
        this.session = session;
        this.customerCodeService = customerCodeService;
        this.loginUserService = loginUserService;
        this.writeOffFeeService = writeOffFeeService;
        this.logisticsOrderFeeReturnToCustomerService = logisticsOrderFeeReturnToCustomerService;
        this.logisticsOrderService = logisticsOrderService;
    }

    [HttpGet]
    [Route("/admin/fee/write/off/approve")]
    public IActionResult Index()
    {
        LogisticsOrderFeeWriteOffApproveIndexModel model = new LogisticsOrderFeeWriteOffApproveIndexModel();

        model.FeeReasonList = session.Query<ManualFeeReasonInfo>().Where(e => e.FeeType == ManualFeeType.WriteOffFee).ToList();
        model.ApplyStatusList = FeeWriteOffApplyStatus.GetApproveViewList();

        model.ApplyUserList = session.Query<FeeWriteOffApplyInfo>()
            .Fetch(e => e.ApplyUser)
            .OrderBy(e => e.ApplyUser.LoginName)
            .Select(e => e.ApplyUser)
            .Distinct()
            .ToList();

        return View("LogisticsOrderFeeWriteOffApproveIndex", model);
    }

    [HttpPost]
    [Route("/admin/fee/write/off/approve/list")]
    public IActionResult List(LogisticsOrderFeeWriteOffApproveListQueryParam queryParam)
    {
        int totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<FeeWriteOffApplyInfo> logisticsOrderFeeWriteOffApplyList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<FeeWriteOffApplyInfo>();

        IList<int> orderIdList = logisticsOrderFeeWriteOffApplyList.Select(e => e.Order?.Id ?? 0).ToList();

        /*
        IList<FeeWriteOffApplyCountInfo> applyCountList = session
            .Query<FeeWriteOffApplyCountInfo>()
            .Where(e => orderIdList.Contains(e.OrderId))
            .ToList();

        foreach (FeeWriteOffApplyInfo apply in logisticsOrderFeeWriteOffApplyList)
        {
            int orderId = apply.Order?.Id ?? 0;
            apply.OrderApplyCountCache = applyCountList.Where(e => e.OrderId == orderId).Take(1).SingleOrDefault()?.ApplyCount ?? 0;
        }
        */

        IDictionary<int, int> dictApplyCount = logisticsOrderFeeReturnToCustomerService.GetApplyCount(orderIdList);

        foreach (FeeWriteOffApplyInfo apply in logisticsOrderFeeWriteOffApplyList)
        {
            int orderId = apply.Order?.Id ?? 0;
            if (dictApplyCount.TryGetValue(orderId, out int applyCount))
            {
                apply.OrderApplyCountCache = applyCount;
            }
        }

        LogisticsOrderFeeWriteOffApproveListModel model = new LogisticsOrderFeeWriteOffApproveListModel(listPager);
        model.FeeWriteOffApplyList = logisticsOrderFeeWriteOffApplyList;

        return View("LogisticsOrderFeeWriteOffApproveList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(LogisticsOrderFeeWriteOffApproveListQueryParam queryParam)
    {
        ICriteria criteria = session.CreateCriteria<FeeWriteOffApplyInfo>();

        criteria.SetReadOnly(true);

        criteria = criteria.CreateAlias(nameof(FeeWriteOffApplyInfo.Order), nameof(FeeWriteOffApplyInfo.Order));
        criteria = criteria.CreateAlias(nameof(FeeWriteOffApplyInfo.Customer), nameof(FeeWriteOffApplyInfo.Customer));

        criteria = criteria.Add(Restrictions.Where<FeeWriteOffApplyInfo>(e => e.Status != FeeWriteOffApplyStatus.Saved && e.Status != FeeWriteOffApplyStatus.Backed));

        if (queryParam.ManualReasonIdQuery != 0)
        {
            criteria = criteria.Add(Restrictions.Where<FeeWriteOffApplyInfo>(e => e.WriteOffReasonId == queryParam.ManualReasonIdQuery));
        }

        if (string.IsNullOrWhiteSpace(queryParam.OrderCodeQuery) == false)
        {
            string orderCodeQuery = queryParam.OrderCodeQuery.Trim();
            criteria = criteria.Add(Restrictions.Like($"{nameof(FeeWriteOffApplyInfo.Order)}.{nameof(FeeWriteOffApplyInfo.Order.Code)}", orderCodeQuery, MatchMode.Anywhere));
        }

        if (string.IsNullOrWhiteSpace(queryParam.CustomerCodeQuery) == false)
        {
            int customerIdQuery = customerCodeService.GetCustomerIdFromCustomerCode(queryParam.CustomerCodeQuery);
            criteria = criteria.Add(Restrictions.Eq($"{nameof(FeeWriteOffApplyInfo.Customer)}.{nameof(FeeWriteOffApplyInfo.Customer.Id)}", customerIdQuery));
        }

        if (queryParam.StatusQuery != null && queryParam.StatusQuery != FeeWriteOffApplyStatus.None)
        {
            criteria = criteria.Add(Restrictions.Where<FeeWriteOffApplyInfo>(e => e.Status == queryParam.StatusQuery));
        }

        int applyUserIdQuery = queryParam.ApplyUserIdQuery;
        if (applyUserIdQuery != 0)
        {
            criteria = criteria.Add(Restrictions.Where<FeeWriteOffApplyInfo>(e => e.ApplyUser.Id == applyUserIdQuery));
        }

        return criteria;
    }

    [HttpPost]
    [Route("/admin/fee/write/off/approve/detail/{id:int?}")]
    public IActionResult Detail(int? id)
    {
        session.DefaultReadOnly = true;

        FeeWriteOffApplyInfo logisticsOrderFeeWriteOffApply = null;

        if (id == null || id.Value == 0)
        {
            logisticsOrderFeeWriteOffApply = new FeeWriteOffApplyInfo();
        }
        else
        {
            logisticsOrderFeeWriteOffApply = session.Get<FeeWriteOffApplyInfo>(id.Value);
            if (logisticsOrderFeeWriteOffApply == null)
            {
                throw new ValidationException("指定的数据项不存在！");
            }
        }

        LogisticsOrderFeeWriteOffApproveDetailModel model = new LogisticsOrderFeeWriteOffApproveDetailModel();
        model.FeeWriteOffApply = logisticsOrderFeeWriteOffApply;

        return View("LogisticsOrderFeeWriteOffApproveDetail", model);
    }

    [HttpPost]
    [Route("/admin/fee/write/off/approve/back")]
    [Transactional]
    public IActionResult Back(int[] idList)
    {
        if (idList == null || idList.Length == 0)
        {
            throw new ValidationException("请选择要回退的数据项！");
        }

        IList<FeeWriteOffApplyInfo> logisticsOrderFeeWriteOffApplyList = session
            .Query<FeeWriteOffApplyInfo>()
            .Where(e => idList.Contains(e.Id))
            .ToList();

        foreach (FeeWriteOffApplyInfo apply in logisticsOrderFeeWriteOffApplyList)
        {
            if (apply.Status != FeeWriteOffApplyStatus.Submitted)
            {
                throw new ValidationException("只能回退【已提交】的申请！");
            }
        }

        foreach (FeeWriteOffApplyInfo apply in logisticsOrderFeeWriteOffApplyList)
        {
            apply.Status = FeeWriteOffApplyStatus.Backed;
            apply.ApproveUser = session.Get<UserInfo>(loginUserService.GetLoginUserId());
            session.Update(apply);
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "回退成功！" },
        });
    }

    [HttpPost]
    [Route("/admin/fee/write/off/approve/approve")]
    [Transactional]
    public IActionResult Approve(int[] idList)
    {
        if (idList == null || idList.Length == 0)
        {
            throw new ValidationException("请选择要审核的数据项！");
        }

        IList<FeeWriteOffApplyInfo> applyList = null;

        ITransaction transaction = null;
        try
        {
            transaction = session.BeginTransaction();

            applyList = session
                .Query<FeeWriteOffApplyInfo>()
                .Fetch(e => e.Order)
                .Fetch(e => e.Customer)
                .Where(e => idList.Contains(e.Id))
                .ToList();

            foreach (FeeWriteOffApplyInfo apply in applyList)
            {
                if (apply.Status != FeeWriteOffApplyStatus.Submitted)
                {
                    throw new ValidationException("只能审核【已提交】的申请！");
                }
            }

            foreach (FeeWriteOffApplyInfo apply in applyList)
            {
                writeOffFeeService.AddWriteOffFee(apply.Order, apply.Customer, apply.Amount, apply.WriteOffReasonId);
            }

            foreach (FeeWriteOffApplyInfo apply in applyList)
            {
                apply.Status = FeeWriteOffApplyStatus.Approved;
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
            foreach (FeeWriteOffApplyInfo apply in applyList)
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