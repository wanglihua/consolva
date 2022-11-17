using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using shunda.Admin.Areas.Fee.Models.ManualIncreaseFee;
using shunda.Admin.Services;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Customer;
using shunda.Domain.Fee;
using shunda.Domain.Order;
using shunda.Domain.Pass.PassRes;
using shunda.Service.Customer;
using shunda.Service.Fee;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Fee.Controllers;

[FeeArea]
[AdminAuth(PassRes.Fee.ManualIncreaseFee)]
public class ManualIncreaseFeeController : Controller
{
    private readonly ISession session;
    private readonly LoginUserService loginUserService;
    private readonly CustomerCodeService customerCodeService;
    private readonly ManualFeeService manualFeeService;

    public ManualIncreaseFeeController(ISession session,
        CustomerCodeService customerCodeService,
        LoginUserService loginUserService,
        ManualFeeService manualFeeService)
    {
        this.session = session;
        this.loginUserService = loginUserService;
        this.customerCodeService = customerCodeService;
        this.manualFeeService = manualFeeService;
    }

    [HttpGet]
    [Route("/admin/manual/increase/fee")]
    public IActionResult Index()
    {
        ManualIncreaseFeeIndexModel model = new ManualIncreaseFeeIndexModel();

        model.FeeReasonList = session.Query<ManualFeeReasonInfo>().Where(e => e.FeeType == ManualFeeType.ManualIncrease).ToList();

        return View("ManualIncreaseFeeIndex", model);
    }

    [HttpPost]
    [Route("/admin/manual/increase/fee/list")]
    public IActionResult List(ManualIncreaseFeeListQueryParam queryParam)
    {
        int totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<LogisticsOrderFeeInfo> logisticsOrderFeeList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<LogisticsOrderFeeInfo>();

        foreach (LogisticsOrderFeeInfo orderFee in logisticsOrderFeeList)
        {
            orderFee.CustomerCodeCache = customerCodeService.GetCustomerCode(orderFee.Customer.Id);
        }

        ManualIncreaseFeeListModel model = new ManualIncreaseFeeListModel(listPager);
        model.LogisticsOrderFeeList = logisticsOrderFeeList;

        return View("ManualIncreaseFeeList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(ManualIncreaseFeeListQueryParam queryParam)
    {
        ICriteria criteria = session.CreateCriteria<LogisticsOrderFeeInfo>();

        // criteria = criteria.CreateAlias(nameof(LogisticsOrderFeeInfo.Order), nameof(LogisticsOrderFeeInfo.Order));
        // criteria = criteria.CreateAlias(nameof(LogisticsOrderFeeInfo.Customer), nameof(LogisticsOrderFeeInfo.Customer));

        criteria = criteria.Add(Restrictions.Where<LogisticsOrderFeeInfo>(e => e.Type == LogisticsOrderFeeType.ManualIncrease));

        criteria = criteria.SetReadOnly(true);

        if (queryParam.ManualReasonIdQuery != 0)
        {
            criteria = criteria.Add(Restrictions.Where<LogisticsOrderFeeInfo>(e => e.ManualReason!.Id == queryParam.ManualReasonIdQuery));
        }

        if (string.IsNullOrWhiteSpace(queryParam.OrderCodeQuery) == false)
        {
            string orderCodeQuery = queryParam.OrderCodeQuery.Trim();
            criteria = criteria.Add(Restrictions.Eq($"{nameof(LogisticsOrderFeeInfo.Order)}.{nameof(LogisticsOrderFeeInfo.Order.Code)}", orderCodeQuery));
        }

        if (string.IsNullOrWhiteSpace(queryParam.CustomerCodeQuery) == false)
        {
            int customerIdQuery = customerCodeService.GetCustomerIdFromCustomerCode(queryParam.CustomerCodeQuery);
            criteria = criteria.Add(Restrictions.Eq($"{nameof(LogisticsOrderFeeInfo.Customer)}.{nameof(LogisticsOrderFeeInfo.Customer.Id)}", customerIdQuery));
        }

        return criteria;
    }

    [HttpPost]
    [Route("/admin/manual/increase/fee/detail")]
    public IActionResult Detail()
    {
        session.DefaultReadOnly = true;

        ManualIncreaseFeeDetailModel model = new ManualIncreaseFeeDetailModel();
        model.FeeReasonList = session.Query<ManualFeeReasonInfo>().Where(e => e.FeeType == ManualFeeType.ManualIncrease).ToList();

        return View("ManualIncreaseFeeDetail", model);
    }

    [HttpPost]
    [Route("/admin/manual/increase/fee/save")]
    [Transactional]
    public IActionResult Save(ManualIncreaseFeeSaveModel model)
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

        CustomerInfo? customer = session.Get<CustomerInfo>(customerId);
        if (customer == null)
        {
            throw new ValidationException("指定的客户不存在！");
        }

        if (logisticsOrder.Customer.Id != customer.Id)
        {
            throw new ValidationException("指定的订单和客户不匹配！");
        }

        if (model.Fee < 0)
        {
            throw new ValidationException("手工增加费用不能小于0！");
        }

        int loginUserId = loginUserService.GetLoginUserId();
        manualFeeService.AddManualIncreaseFee(logisticsOrder, customer, model.Fee, model.ManualReasonId, model.Description ?? string.Empty, loginUserId);

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "保存成功！" }
        });
    }
}