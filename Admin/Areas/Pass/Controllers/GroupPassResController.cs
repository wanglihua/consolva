using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using shunda.Admin.Areas.Pass.Models.GroupPassRes;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Web;
using shunda.Domain.Pass;
using shunda.Service.Pass;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Pass.Controllers;

[AdminAuth(AdminOnly = true)]
[PassArea]
public class GroupPassResController : Controller
{
    private static readonly string passResCateAlias = "prc";
    private readonly PassResService passResService;
    private readonly ISession session;

    public GroupPassResController(ISession session, PassResService passResService)
    {
        this.session = session;
        this.passResService = passResService;
    }

    [HttpGet]
    [Route("/admin/group/pass/res/{groupId:int}")]
    public IActionResult Index(int groupId)
    {
        var group = session.Get<GroupInfo>(groupId);
        if (group == null) return Content("指定的群组 Id 不存在！");

        var model = new GroupPassResIndexModel();
        model.GroupId = groupId;
        model.Group = group;

        return View("GroupPassResIndex", model);
    }

    [HttpPost]
    [Route("/admin/group/pass/res/list")]
    public IActionResult List(GroupPassResListQueryParam queryParam)
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

        var groupId = queryParam.GroupId;
        IList<int> grantedPassResIdList = session.Query<GroupPassResInfo>().Where(e => e.Group.Id == groupId).Select(e => e.PassRes.Id).ToList();
        IDictionary<int, bool> grantedPassResIdMap = new Dictionary<int, bool>();
        foreach (var passResId in grantedPassResIdList) grantedPassResIdMap[passResId] = true;

        IList<GroupPassResListItem> groupPassResItemList = new List<GroupPassResListItem>();
        foreach (var passResInfo in passResList)
        {
            var groupPassResListItem = new GroupPassResListItem();
            groupPassResListItem.PassResId = passResInfo.Id;
            groupPassResListItem.CateName = passResInfo.Cate.Name;
            groupPassResListItem.ResName = passResInfo.Name;

            groupPassResListItem.Granted = false;
            if (grantedPassResIdMap.ContainsKey(passResInfo.Id)) groupPassResListItem.Granted = true;

            groupPassResItemList.Add(groupPassResListItem);
        }

        var model = new GroupPassResListModel(listPager);
        model.GroupPassResList = groupPassResItemList;
        model.PassResCateAlias = passResCateAlias;

        return View("GroupPassResList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(GroupPassResListQueryParam queryParam)
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
    [Route("/admin/group/pass/res/grant")]
    [Transactional]
    public IActionResult Grant(GroupPassResGrantModel model)
    {
        var passResId = model.PassResId;
        var groupId = model.GroupId;

        var count = session.Query<GroupPassResInfo>().Where(e => e.PassRes.Id == passResId && e.Group.Id == groupId).Count();
        if (count == 0)
        {
            var groupPassResInfo = new GroupPassResInfo();
            groupPassResInfo.PassRes = session.Load<PassResInfo>(passResId);
            groupPassResInfo.Group = session.Load<GroupInfo>(groupId);

            session.Save(groupPassResInfo);
            session.Flush();
        }

        passResService.CachePassResIdMapForAllUser();

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "message", "授权成功！" }
        });
    }

    [HttpPost]
    [Route("/admin/group/pass/res/revoke")]
    [Transactional]
    public IActionResult Revoke(GroupPassResRevokeModel model)
    {
        var passResId = model.PassResId;
        var groupId = model.GroupId;

        session.Query<GroupPassResInfo>().Where(e => e.PassRes.Id == passResId && e.Group.Id == groupId).Delete();
        session.Flush();

        passResService.CachePassResIdMapForAllUser();

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "message", "收权成功！" }
        });
    }
}