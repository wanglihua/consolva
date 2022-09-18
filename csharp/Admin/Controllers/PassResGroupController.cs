using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using shunda.Admin.Models.PassResGroup;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Pass;
using shunda.Service;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Controllers;

[AdminAuth(AdminOnly = true)]
public class PassResGroupController : Controller
{
    private ISession session;
    private PassResService passResService;

    public PassResGroupController(ISession session, PassResService passResService)
    {
        this.session = session;
        this.passResService = passResService;
    }

    [HttpGet]
    [Route("/admin/pass/res/group/{passResId:int}")]
    public IActionResult Index(int passResId)
    {
        PassResInfo passRes = this.session.Get<PassResInfo>(passResId);
        if (passRes == null)
        {
            return Content("指定的资源不存在！");
        }

        PassResGroupIndexModel model = new PassResGroupIndexModel();
        model.PassResId = passResId;
        model.PassRes = passRes;

        return View("PassResGroupIndex", model);
    }

    [HttpPost]
    [Route("/admin/pass/res/list/group")]
    public IActionResult List(PassResGroupListQueryParam queryParam)
    {
        int totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<GroupPassResInfo> groupPassResList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<GroupPassResInfo>();

        PassResGroupListModel model = new PassResGroupListModel(listPager);
        model.GroupPassResList = groupPassResList;
        model.GroupAlias = groupAlias;

        return View("PassResGroupList", model);
    }

    private static readonly string groupAlias = "g";

    [NonAction]
    private ICriteria GetListCriteria(PassResGroupListQueryParam queryParam)
    {
        int passResId = queryParam.PassResId;

        ICriteria criteria = this.session.CreateCriteria<GroupPassResInfo>();

        criteria = criteria.CreateAlias(nameof(GroupPassResInfo.Group), groupAlias);

        criteria = criteria.Add(Restrictions.Where<GroupPassResInfo>(e => e.PassRes.Id == passResId));

        criteria = criteria.SetReadOnly(true);
        
        return criteria;
    }

    [HttpPost]
    [Route("/admin/pass/res/group/add/{passResId:int}")]
    public IActionResult Add(int passResId)
    {
        this.session.DefaultReadOnly = true;

        PassResInfo passRes = this.session.Get<PassResInfo>(passResId);
        if (passRes == null)
        {
            throw new ValidationException("指定的资源不存在！");
        }

        PassResGroupAddModel model = new PassResGroupAddModel();
        model.PassResId = passResId;
        model.PassRes = passRes;

        return View("PassResGroupAdd", model);
    }

    [HttpPost]
    [Route("/admin/pass/res/group/add/save")]
    [Transactional]
    public IActionResult AddSave(PassResGroupAddModel model)
    {
        if (!this.ModelState.IsValid)
        {
            throw new ValidationException(ValidationUtil.GetErrorMessages(this.ModelState));
        }

        int passResId = model.PassResId;
        PassResInfo passRes = this.session.Get<PassResInfo>(passResId);
        if (passRes == null)
        {
            throw new ValidationException("指定的资源不存在！");
        }

        string groupCode = model.GroupCode;

        GroupInfo? group = this.session.Query<GroupInfo>()
            .Where(e => e.Code == groupCode)
            .Take(1)
            .SingleOrDefault();
        if (group == null)
        {
            throw new ValidationException("指定的群组不存在！");
        }

        int groupId = group.Id;

        int groupPassResCount = this.session.Query<GroupPassResInfo>().Where(e => e.PassRes.Id == passResId && e.Group.Id == groupId).Count();
        if (groupPassResCount != 0)
        {
            throw new ValidationException("该资源已经授权给指定群组！");
        }

        GroupPassResInfo groupPassRes = new GroupPassResInfo();
        groupPassRes.Group = this.session.Load<GroupInfo>(groupId);
        groupPassRes.PassRes = this.session.Load<PassResInfo>(passResId);

        this.session.Save(groupPassRes);
        this.session.Flush();

        passResService.CachePassResIdMapForAllUser();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "保存成功！" }
        });
    }

    [HttpPost]
    [Route("/admin/pass/res/group/remove")]
    [Transactional]
    public IActionResult Remove(int[] idList)
    {
        foreach (int id in idList)
        {
            GroupPassResInfo groupPassRes = this.session.Load<GroupPassResInfo>(id);
            this.session.Delete(groupPassRes);
        }

        this.session.Flush();

        passResService.CachePassResIdMapForAllUser();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "删除成功！" },
        });
    }
}