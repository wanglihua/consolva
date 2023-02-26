using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NLog;
using shunda.Admin.Areas.Express.Models.ExpressExpect;
using shunda.Core;
using shunda.Core.Web;
using shunda.Domain.Customer;
using shunda.Domain.Express;
using shunda.Domain.Pass.PassRes;
using shunda.Service.Customer;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Express.Controllers;

/// <summary>
/// 快递单号预报 控制器
/// </summary>
[ExpressArea]
[AdminAuth(PassRes.Express.ExpressExpect)]
public class ExpressExpectController : Controller
{
    private static readonly Logger log = LogManager.GetCurrentClassLogger();
    private CustomerInfo? customerAlias = null;
    private readonly CustomerCodeService customerCodeService;

    private ExpressExpectInfo? expressExpectAlias = null;

    private readonly ISession session;

    public ExpressExpectController(ISession session, CustomerCodeService customerCodeService)
    {
        this.session = session;
        this.customerCodeService = customerCodeService;
    }

    [HttpGet]
    [Route("/admin/express/express/expect")]
    public IActionResult Index()
    {
        var model = new ExpressExpectIndexModel();

        model.StatusList = ExpressExpectStatus.GetList();

        return View("ExpressExpectIndex", model);
    }

    [HttpPost]
    [Route("/admin/express/express/expect/list")]
    public IActionResult List(ExpressExpectListQueryParam queryParam)
    {
        var totalCount = GetQueryOver(queryParam)
            // .Select(Projections.Count<ExpressExpectInfo>(e => expressExpectAlias!.Id))
            .Select(Projections.RowCount())
            .SingleOrDefault<int>();

        var listPager = new ListPager(queryParam, totalCount);

        var listQuery = GetQueryOver(queryParam);
        if (listPager.IsAscending)
            listQuery = listQuery.OrderBy(Projections.Property(listPager.SortField)).Asc;
        else
            listQuery = listQuery.OrderBy(Projections.Property(listPager.SortField)).Desc;

        listQuery = listQuery.ThenByAlias(() => expressExpectAlias!.Id).Desc;

        var expressExpectList = listQuery
            .Skip(listPager.Skip())
            .Take(listPager.Take())
            .List<ExpressExpectInfo>();

        foreach (var expressExpect in expressExpectList) expressExpect.CustomerCodeCache = customerCodeService.GetCustomerCode(expressExpect.Customer.Id);

        var model = new ExpressExpectListModel(listPager);
        model.ExpressExpectList = expressExpectList;

        model.ExpressExpectAlias = nameof(expressExpectAlias);
        model.CustomerAlias = nameof(customerAlias);

        // todo e 列表中加上客户和已录入快递单号的详细界面的链接URL
        return View("ExpressExpectList", model);
    }

    [NonAction]
    private IQueryOver<ExpressExpectInfo, ExpressExpectInfo> GetQueryOver(ExpressExpectListQueryParam queryParam)
    {
        var query = session.QueryOver(() => expressExpectAlias!);
        // .JoinAlias(() => expressExpectAlias!.Customer, () => customerAlias); 不加这个感觉也可以

        if (string.IsNullOrWhiteSpace(queryParam.CustomerCodeQuery) == false)
        {
            var customerId = customerCodeService.GetCustomerIdFromCustomerCode(queryParam.CustomerCodeQuery.Trim());
            query = query.Where(e => expressExpectAlias!.Customer.Id == customerId);
        }

        if (string.IsNullOrWhiteSpace(queryParam.TrackingNoQuery) == false)
        {
            var trackingNoQuery = queryParam.TrackingNoQuery.Trim();
            query = query.Where(e => expressExpectAlias!.TrackingNo == trackingNoQuery);
        }

        return query;
    }
}