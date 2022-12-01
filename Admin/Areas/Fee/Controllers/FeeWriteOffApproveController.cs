using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using NLog;
using shunda.Admin.Areas.Fee.Models.FeeWriteOffApprove;
using shunda.Admin.Services;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Web;
using shunda.Domain.Fee;
using shunda.Domain.Pass;
using shunda.Domain.Pass.PassRes;
using shunda.Service.Customer;
using shunda.Service.Fee;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Fee.Controllers;

[FeeArea]
[AdminAuth(PassRes.Fee.WriteOffFeeApprove)]
public class FeeWriteOffApproveController : Controller
{
    private static Logger log = LogManager.GetCurrentClassLogger();

    private ISession session;
    private CustomerCodeService customerCodeService;
    private LoginUserService loginUserService;
    private WriteOffFeeService writeOffFeeService;
    private FeeApplyCountService feeApplyCountService;

    public FeeWriteOffApproveController(ISession session,
        CustomerCodeService customerCodeService,
        LoginUserService loginUserService,
        WriteOffFeeService writeOffFeeService,
        FeeApplyCountService feeApplyCountService)
    {
        this.session = session;
        this.customerCodeService = customerCodeService;
        this.loginUserService = loginUserService;
        this.writeOffFeeService = writeOffFeeService;
        this.feeApplyCountService = feeApplyCountService;
    }

    [HttpGet]
    [Route("/admin/fee/write/off/approve")]
    public IActionResult Index()
    {
        FeeWriteOffApproveIndexModel model = new FeeWriteOffApproveIndexModel();

        model.FeeReasonList = session.Query<ManualFeeReasonInfo>().Where(e => e.FeeType == ManualFeeType.WriteOffFee).ToList();
        model.ApplyStatusList = FeeWriteOffApplyStatus.GetApproveViewList();

        model.ApplyUserList = session.Query<FeeWriteOffApplyInfo>()
            .Fetch(e => e.ApplyUser)
            .OrderBy(e => e.ApplyUser.LoginName)
            .Select(e => e.ApplyUser)
            .Distinct()
            .ToList();

        return View("FeeWriteOffApproveIndex", model);
    }

    [HttpPost]
    [Route("/admin/fee/write/off/approve/list")]
    public IActionResult List(FeeWriteOffApproveListQueryParam queryParam)
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

        IDictionary<int, int> dictApplyCount = feeApplyCountService.GetApplyCount(orderIdList);

        foreach (FeeWriteOffApplyInfo apply in logisticsOrderFeeWriteOffApplyList)
        {
            int orderId = apply.Order?.Id ?? 0;
            if (dictApplyCount.TryGetValue(orderId, out int applyCount))
            {
                apply.OrderApplyCountCache = applyCount;
            }

            apply.CustomerCodeCache = customerCodeService.GetCustomerCode(apply.Customer.Id);
        }

        FeeWriteOffApproveListModel model = new FeeWriteOffApproveListModel(listPager);
        model.FeeWriteOffApplyList = logisticsOrderFeeWriteOffApplyList;

        return View("FeeWriteOffApproveList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(FeeWriteOffApproveListQueryParam queryParam)
    {
        ICriteria criteria = session.CreateCriteria<FeeWriteOffApplyInfo>();

        criteria.SetReadOnly(true);

        // criteria = criteria.CreateAlias(nameof(FeeWriteOffApplyInfo.Order), nameof(FeeWriteOffApplyInfo.Order));
        // criteria = criteria.CreateAlias(nameof(FeeWriteOffApplyInfo.Customer), nameof(FeeWriteOffApplyInfo.Customer));

        criteria = criteria.Add(Restrictions.Where<FeeWriteOffApplyInfo>(e => e.Status != FeeWriteOffApplyStatus.Saved && e.Status != FeeWriteOffApplyStatus.Backed));

        if (queryParam.ManualReasonIdQuery != 0)
        {
            criteria = criteria.Add(Restrictions.Where<FeeWriteOffApplyInfo>(e => e.WriteOffReason.Id == queryParam.ManualReasonIdQuery));
        }

        if (string.IsNullOrWhiteSpace(queryParam.OrderCodeQuery) == false)
        {
            string orderCodeQuery = queryParam.OrderCodeQuery.Trim();
            criteria = criteria.Add(Restrictions.Eq($"{nameof(FeeWriteOffApplyInfo.Order)}.{nameof(FeeWriteOffApplyInfo.Order.Code)}", orderCodeQuery));
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

    /// <summary>
    /// 这个现在好像还没用起来
    /// </summary>
    [HttpPost]
    [Route("/admin/fee/write/off/approve/detail/{id:int?}")]
    public IActionResult Detail(int? id)
    {
        session.DefaultReadOnly = true;

        FeeWriteOffApplyInfo? feeWriteOffApply;

        if (id == null || id.Value == 0)
        {
            feeWriteOffApply = new FeeWriteOffApplyInfo();
        }
        else
        {
            feeWriteOffApply = session.Get<FeeWriteOffApplyInfo>(id.Value);
            if (feeWriteOffApply == null)
            {
                throw new ValidationException("指定的数据项不存在！");
            }
        }

        FeeWriteOffApproveDetailModel model = new FeeWriteOffApproveDetailModel();
        model.FeeWriteOffApply = feeWriteOffApply;

        return View("FeeWriteOffApproveDetail", model);
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

        int loginUserId = loginUserService.GetLoginUserId();

        IList<FeeWriteOffApplyInfo> applyList = session
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
            writeOffFeeService.AddWriteOffFee(apply.Order, apply.Customer, apply.Amount, apply.WriteOffReason.Id, loginUserId);
        }

        foreach (FeeWriteOffApplyInfo apply in applyList)
        {
            apply.Status = FeeWriteOffApplyStatus.Approved;
            apply.ApproveUser = session.Get<UserInfo>(loginUserService.GetLoginUserId());
            session.Update(apply);
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "审核成功！" },
        });
    }
}