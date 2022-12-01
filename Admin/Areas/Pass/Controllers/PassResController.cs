using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using shunda.Admin.Areas.Pass.Models.PassRes;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Web;
using shunda.Domain.Pass;
using shunda.Service.Pass;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Pass.Controllers;

[AdminAuth(AdminOnly = true)]
[PassArea]
public class PassResController : Controller
{
    private static readonly string passResCateAlias = "prc";
    private readonly PassResCateDataInitService passResCateDataInitService;
    private readonly PassResDataInitService passResDataInitService;
    private readonly PassResService passResService;
    private readonly ISession session;

    public PassResController(ISession session,
        PassResCateDataInitService passResCateDataInitService,
        PassResDataInitService passResDataInitService,
        PassResService passResService)
    {
        this.session = session;
        this.passResCateDataInitService = passResCateDataInitService;
        this.passResDataInitService = passResDataInitService;
        this.passResService = passResService;
    }

    [HttpGet]
    [Route("/admin/pass/res")]
    public IActionResult Index()
    {
        var model = new PassResIndexModel();

        model.CateList = session.Query<PassResCateInfo>()
            .OrderBy(e => e.Id)
            .ToList();

        model.PassResCateAlias = passResCateAlias;

        return View("PassResIndex", model);
    }

    [HttpPost]
    [Route("/admin/pass/res/list")]
    public IActionResult List(PassResListQueryParam queryParam)
    {
        var totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        var listPager = new ListPager(queryParam, totalCount);

        var passResList = GetListCriteria(queryParam)
            .AddOrder(Order.Asc($"{passResCateAlias}.{nameof(PassResCateInfo.OrderNum)}"))
            .AddOrder(Order.Asc($"{passResCateAlias}.{nameof(PassResCateInfo.Id)}"))
            .AddOrder(Order.Asc(nameof(PassResInfo.OrderNum)))
            .AddOrder(Order.Asc(nameof(PassResInfo.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<PassResInfo>();

        var model = new PassResListModel(listPager);
        model.PassResList = passResList;
        model.PassResCateAlias = passResCateAlias;

        return View("PassResList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(PassResListQueryParam queryParam)
    {
        var criteria = session.CreateCriteria<PassResInfo>();
        criteria = criteria.CreateAlias(nameof(PassResInfo.Cate), passResCateAlias);

        criteria = criteria.SetReadOnly(true);

        var cateIdQuery = queryParam.CateIdQuery;
        if (cateIdQuery != 0) criteria = criteria.Add(Restrictions.Where<PassResInfo>(e => e.Cate.Id == cateIdQuery));

        if (string.IsNullOrWhiteSpace(queryParam.NameQuery) == false)
        {
            var nameQuery = queryParam.NameQuery.Trim();
            criteria = criteria.Add(Restrictions.On<PassResInfo>(e => e.Name).IsInsensitiveLike(nameQuery, MatchMode.Anywhere));
        }

        return criteria;
    }

    [HttpPost]
    [Route("/admin/pass/res/init/data")]
    [Transactional]
    public IActionResult InitData()
    {
        passResCateDataInitService.InitData();
        passResDataInitService.InitData();

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "message", "数据初始化成功！" }
        });
    }

    [HttpPost]
    [Route("/admin/pass/res/refresh/cache")]
    public IActionResult RefreshCache()
    {
        passResService.CachePassResIdMapForAllUser();

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "message", "权限资源授权数据缓存刷新成功！" }
        });
    }
}