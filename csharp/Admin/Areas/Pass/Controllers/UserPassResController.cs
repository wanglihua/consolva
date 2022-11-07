using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using shunda.Admin.Areas.Pass.Models.UserPassRes;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Web;
using shunda.Domain.Pass;
using shunda.Service.Pass;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Pass.Controllers;

[AdminAuth(AdminOnly = true)]
[PassArea]
public class UserPassResController : Controller
{
    private static readonly string passResCateAlias = "prc";
    private readonly PassResService passResService;
    private readonly ISession session;

    public UserPassResController(ISession session, PassResService passResService)
    {
        this.session = session;
        this.passResService = passResService;
    }

    [HttpGet]
    [Route("/admin/user/pass/res/{userId:int}")]
    public IActionResult Index(int userId)
    {
        var user = session.Get<UserInfo>(userId);
        if (user == null) return Content("指定的用户 Id 不存在！");

        var model = new UserPassResIndexModel();
        model.UserId = userId;
        model.User = user;

        return View("UserPassResIndex", model);
    }

    [HttpPost]
    [Route("/admin/user/pass/res/list")]
    public IActionResult List(UserPassResListQueryParam queryParam)
    {
        var totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        var listPager = new ListPager(queryParam, totalCount);

        var passResList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<PassResInfo>();

        var userId = queryParam.UserId;
        IList<int> grantedPassResIdList = session.Query<UserPassResInfo>().Where(e => e.User.Id == userId).Select(e => e.PassRes.Id).ToList();
        IDictionary<int, bool> grantedPassResIdMap = new Dictionary<int, bool>();
        foreach (var passResId in grantedPassResIdList) grantedPassResIdMap[passResId] = true;

        IList<UserPassResListItem> userPassResItemList = new List<UserPassResListItem>();
        foreach (var passResInfo in passResList)
        {
            var userPassResListItem = new UserPassResListItem();
            userPassResListItem.PassResId = passResInfo.Id;
            userPassResListItem.CateName = passResInfo.Cate.Name;
            userPassResListItem.ResName = passResInfo.Name;

            userPassResListItem.Granted = false;
            if (grantedPassResIdMap.ContainsKey(passResInfo.Id)) userPassResListItem.Granted = true;

            userPassResItemList.Add(userPassResListItem);
        }

        var model = new UserPassResListModel(listPager);
        model.UserPassResList = userPassResItemList;
        model.PassResCateAlias = passResCateAlias;

        return View("UserPassResList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(UserPassResListQueryParam queryParam)
    {
        var criteria = session.CreateCriteria<PassResInfo>();
        criteria = criteria.CreateAlias(nameof(PassResInfo.Cate), passResCateAlias);

        criteria = criteria.SetReadOnly(true);

        if (string.IsNullOrWhiteSpace(queryParam.PassResNameQuery) == false)
        {
            var passResNameQuery = queryParam.PassResNameQuery.Trim();
            criteria = criteria.Add(Restrictions.On<PassResInfo>(e => e.Name).IsInsensitiveLike(passResNameQuery, MatchMode.Anywhere));
        }

        if (string.IsNullOrWhiteSpace(queryParam.CateNameQuery) == false)
        {
            var cateNameQuery = queryParam.CateNameQuery.Trim();
            criteria.Add(Restrictions.InsensitiveLike($"{passResCateAlias}.{nameof(PassResCateInfo.Name)}", cateNameQuery, MatchMode.Anywhere));
        }

        return criteria;
    }

    [HttpPost]
    [Route("/admin/user/pass/res/grant")]
    [Transactional]
    public IActionResult Grant(UserPassResGrantModel model)
    {
        var passResId = model.PassResId;
        var userId = model.UserId;

        var count = session.Query<UserPassResInfo>().Where(e => e.PassRes.Id == passResId && e.User.Id == userId).Count();
        if (count == 0)
        {
            var userPassResInfo = new UserPassResInfo();
            userPassResInfo.PassRes = session.Load<PassResInfo>(passResId);
            userPassResInfo.User = session.Load<UserInfo>(userId);

            session.Save(userPassResInfo);
            session.Flush();
        }

        passResService.CachePassResIdMapForAllUser();

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "message", "授权成功！" }
        });
    }

    [HttpPost]
    [Route("/admin/user/pass/res/revoke")]
    [Transactional]
    public IActionResult Revoke(UserPassResRevokeModel model)
    {
        var passResId = model.PassResId;
        var userId = model.UserId;

        session.Query<UserPassResInfo>().Where(e => e.PassRes.Id == passResId && e.User.Id == userId).Delete();
        session.Flush();

        passResService.CachePassResIdMapForAllUser();

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "message", "收权成功！" }
        });
    }
}