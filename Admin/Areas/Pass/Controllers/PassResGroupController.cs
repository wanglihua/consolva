using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using shunda.Admin.Areas.Pass.Models.PassResGroup;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Pass;
using shunda.Service.Pass;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Pass.Controllers;

[AdminAuth(AdminOnly = true)]
[PassArea]
public class PassResGroupController : Controller
{
    private static readonly string groupAlias = "g";
    private readonly PassResService passResService;
    private readonly ISession session;

    public PassResGroupController(ISession session, PassResService passResService)
    {
        this.session = session;
        this.passResService = passResService;
    }

    [HttpGet]
    [Route("/admin/pass/res/group/{passResId:int}")]
    public IActionResult Index(int passResId)
    {
        var passRes = session.Get<PassResInfo>(passResId);
        if (passRes == null) return Content("指定的资源不存在！");

        var model = new PassResGroupIndexModel();
        model.PassResId = passResId;
        model.PassRes = passRes;

        return View("PassResGroupIndex", model);
    }

    [HttpPost]
    [Route("/admin/pass/res/list/group")]
    public IActionResult List(PassResGroupListQueryParam queryParam)
    {
        var totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        var listPager = new ListPager(queryParam, totalCount);

        var groupPassResList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<GroupPassResInfo>();

        var model = new PassResGroupListModel(listPager);
        model.GroupPassResList = groupPassResList;
        model.GroupAlias = groupAlias;

        return View("PassResGroupList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(PassResGroupListQueryParam queryParam)
    {
        var passResId = queryParam.PassResId;

        var criteria = session.CreateCriteria<GroupPassResInfo>();

        criteria = criteria.CreateAlias(nameof(GroupPassResInfo.Group), groupAlias);

        criteria = criteria.Add(Restrictions.Where<GroupPassResInfo>(e => e.PassRes.Id == passResId));

        criteria = criteria.SetReadOnly(true);

        return criteria;
    }

    [HttpPost]
    [Route("/admin/pass/res/group/add/{passResId:int}")]
    public IActionResult Add(int passResId)
    {
        var passRes = session.Get<PassResInfo>(passResId);
        if (passRes == null) throw new ValidationException("指定的资源不存在！");

        var model = new PassResGroupAddModel();
        model.PassResId = passResId;
        model.PassRes = passRes;

        return View("PassResGroupAdd", model);
    }

    [HttpPost]
    [Route("/admin/pass/res/group/add/save")]
    [Transactional]
    public IActionResult AddSave(PassResGroupAddModel model)
    {
        if (!ModelState.IsValid) throw new ValidationException(ValidationUtil.GetErrorMessages(ModelState));

        var passResId = model.PassResId;
        var passRes = session.Get<PassResInfo>(passResId);
        if (passRes == null) throw new ValidationException("指定的资源不存在！");

        var groupCode = model.GroupCode;

        var group = session.Query<GroupInfo>()
            .Where(e => e.Code == groupCode)
            .Take(1)
            .SingleOrDefault();
        if (group == null) throw new ValidationException("指定的群组不存在！");

        var groupId = group.Id;

        var groupPassResCount = session.Query<GroupPassResInfo>().Where(e => e.PassRes.Id == passResId && e.Group.Id == groupId).Count();
        if (groupPassResCount != 0) throw new ValidationException("该资源已经授权给指定群组！");

        var groupPassRes = new GroupPassResInfo();
        groupPassRes.Group = session.Load<GroupInfo>(groupId);
        groupPassRes.PassRes = session.Load<PassResInfo>(passResId);

        session.Save(groupPassRes);
        session.Flush();

        passResService.CachePassResIdMapForAllUser();

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "message", "保存成功！" }
        });
    }

    [HttpPost]
    [Route("/admin/pass/res/group/remove")]
    [Transactional]
    public IActionResult Remove(int[] idList)
    {
        foreach (var id in idList)
        {
            var groupPassRes = session.Load<GroupPassResInfo>(id);
            session.Delete(groupPassRes);
        }

        session.Flush();

        passResService.CachePassResIdMapForAllUser();

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "message", "删除成功！" }
        });
    }
}