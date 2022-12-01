using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using shunda.Admin.Areas.Fee.Models.ExpressReturnPay;
using shunda.Core;
using shunda.Core.Web;
using shunda.Domain.Fee;
using shunda.Domain.Pass.PassRes;
using shunda.Service.Customer;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Fee.Controllers;

[FeeArea]
[AdminAuth(PassRes.Fee.ExpressReturnPay)]
public class ExpressReturnPayController : Controller
{
    private ISession session;
    private CustomerCodeService customerCodeService;

    public ExpressReturnPayController(ISession session, CustomerCodeService customerCodeService)
    {
        this.session = session;
        this.customerCodeService = customerCodeService;
    }

    [HttpGet]
    [Route("/admin/express/return/pay")]
    public IActionResult Index()
    {
        ExpressReturnPayIndexModel model = new ExpressReturnPayIndexModel();

        model.PayTypeList = PayType.GetList();
        model.StatusList = PayStatus.GetList();

        return View("ExpressReturnPayIndex", model);
    }

    [HttpPost]
    [Route("/admin/express/return/pay/list")]
    public IActionResult List(ExpressReturnPayListQueryParam queryParam)
    {
        int totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<ExpressReturnPayInfo> expressReturnPayList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<ExpressReturnPayInfo>();

        foreach (ExpressReturnPayInfo expressReturnPay in expressReturnPayList)
        {
            expressReturnPay.CustomerCodeCache = customerCodeService.GetCustomerCode(expressReturnPay.Customer.Id);
        }

        ExpressReturnPayListModel model = new ExpressReturnPayListModel(listPager);
        model.ExpressReturnPayList = expressReturnPayList;

        return View("ExpressReturnPayList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(ExpressReturnPayListQueryParam queryParam)
    {
        ICriteria criteria = session.CreateCriteria<ExpressReturnPayInfo>();
        criteria = criteria.SetReadOnly(true);

        criteria = criteria.CreateAlias(nameof(ExpressReturnPayInfo.Express), nameof(ExpressReturnPayInfo.Express));
        // criteria = criteria.CreateAlias(nameof(ExpressReturnPayInfo.Customer), nameof(ExpressReturnPayInfo.Customer));

        criteria = criteria.Add(Restrictions.Where<ExpressReturnPayInfo>(e => e.Status == PayStatus.Paid));

        if (string.IsNullOrWhiteSpace(queryParam.CustomerCodeQuery) == false)
        {
            string customerCodeQuery = queryParam.CustomerCodeQuery.Trim();

            int customerId = customerCodeService.GetCustomerIdFromCustomerCode(customerCodeQuery);
            criteria = criteria.Add(Restrictions.Where<ExpressReturnPayInfo>(e => e.Customer.Id == customerId));
        }

        if (queryParam.PayTimeBeginQuery != null)
        {
            DateTime payTimeBeginQuery = queryParam.PayTimeBeginQuery.Value.Date;
            criteria = criteria.Add(Restrictions.Where<ExpressReturnPayInfo>(e => e.CreateTime >= payTimeBeginQuery));
        }

        if (queryParam.PayTimeEndQuery != null)
        {
            DateTime payTimeEndQuery = queryParam.PayTimeEndQuery.Value.Date;
            payTimeEndQuery = payTimeEndQuery.AddDays(1);

            criteria = criteria.Add(Restrictions.Where<ExpressReturnPayInfo>(e => e.CreateTime < payTimeEndQuery));
        }

        return criteria;
    }
}