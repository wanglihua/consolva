using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using shunda.Admin.Models.UserPassRes;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Web;
using shunda.Domain.Pass;
using shunda.Service;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Controllers;

[AdminAuth(AdminOnly = true)]
public class UserPassResController : Controller
{
    private ISession session;
    private PassResService passResService;

    public UserPassResController(ISession session, PassResService passResService)
    {
        this.session = session;
        this.passResService = passResService;
    }

    [HttpGet]
    [Route("/admin/user/pass/res/{userId:int}")]
    public IActionResult Index(int userId)
    {
        UserInfo user = session.Get<UserInfo>(userId);
        if (user == null)
        {
            return Content("指定的用户 Id 不存在！");
        }

        UserPassResIndexModel model = new UserPassResIndexModel();
        model.UserId = userId;
        model.User = user;

        return View("UserPassResIndex", model);
    }

    [HttpPost]
    [Route("/admin/user/pass/res/list")]
    public IActionResult List(UserPassResListQueryParam queryParam)
    {
        int totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<PassResInfo> passResList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<PassResInfo>();

        int userId = queryParam.UserId;
        IList<int> grantedPassResIdList = session.Query<UserPassResInfo>().Where(e => e.User.Id == userId).Select(e => e.PassRes.Id).ToList();
        IDictionary<int, bool> grantedPassResIdMap = new Dictionary<int, bool>();
        foreach (int passResId in grantedPassResIdList)
        {
            grantedPassResIdMap[passResId] = true;
        }

        IList<UserPassResListItem> userPassResItemList = new List<UserPassResListItem>();
        foreach (PassResInfo passResInfo in passResList)
        {
            UserPassResListItem userPassResListItem = new UserPassResListItem();
            userPassResListItem.PassResId = passResInfo.Id;
            userPassResListItem.CateName = passResInfo.Cate.Name;
            userPassResListItem.ResName = passResInfo.Name;

            userPassResListItem.Granted = false;
            if (grantedPassResIdMap.ContainsKey(passResInfo.Id))
            {
                userPassResListItem.Granted = true;
            }

            userPassResItemList.Add(userPassResListItem);
        }

        UserPassResListModel model = new UserPassResListModel(listPager);
        model.UserPassResList = userPassResItemList;
        model.PassResCateAlias = passResCateAlias;

        return View("UserPassResList", model);
    }

    private static readonly string passResCateAlias = "prc";

    [NonAction]
    private ICriteria GetListCriteria(UserPassResListQueryParam queryParam)
    {
        ICriteria criteria = this.session.CreateCriteria<PassResInfo>();
        criteria = criteria.CreateAlias(nameof(PassResInfo.Cate), passResCateAlias);

        criteria = criteria.SetReadOnly(true);

        if (string.IsNullOrWhiteSpace(queryParam.PassResNameQuery) == false)
        {
            string passResNameQuery = queryParam.PassResNameQuery.Trim();
            criteria = criteria.Add(Restrictions.On<PassResInfo>(e => e.Name).IsInsensitiveLike(passResNameQuery, MatchMode.Anywhere));
        }

        if (string.IsNullOrWhiteSpace(queryParam.CateNameQuery) == false)
        {
            string cateNameQuery = queryParam.CateNameQuery.Trim();
            criteria.Add(Restrictions.InsensitiveLike($"{passResCateAlias}.{nameof(PassResCateInfo.Name)}", cateNameQuery, MatchMode.Anywhere));
        }

        return criteria;
    }

    [HttpPost]
    [Route("/admin/user/pass/res/grant")]
    [Transactional]
    public IActionResult Grant(UserPassResGrantModel model)
    {
        int passResId = model.PassResId;
        int userId = model.UserId;

        int count = session.Query<UserPassResInfo>().Where(e => e.PassRes.Id == passResId && e.User.Id == userId).Count();
        if (count == 0)
        {
            UserPassResInfo userPassResInfo = new UserPassResInfo();
            userPassResInfo.PassRes = session.Load<PassResInfo>(passResId);
            userPassResInfo.User = session.Load<UserInfo>(userId);

            session.Save(userPassResInfo);
            session.Flush();
        }

        passResService.CachePassResIdMapForAllUser();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "授权成功！" },
        });
    }

    [HttpPost]
    [Route("/admin/user/pass/res/revoke")]
    [Transactional]
    public IActionResult Revoke(UserPassResRevokeModel model)
    {
        int passResId = model.PassResId;
        int userId = model.UserId;

        session.Query<UserPassResInfo>().Where(e => e.PassRes.Id == passResId && e.User.Id == userId).Delete();
        session.Flush();

        passResService.CachePassResIdMapForAllUser();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "收权成功！" },
        });
    }
}