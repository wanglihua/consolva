using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using shunda.Admin.Models.GroupMember;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Pass;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Controllers;

[AdminAuth(AdminOnly = true)]
public class GroupMemberController : Controller
{
    private ISession session;

    public GroupMemberController(ISession session)
    {
        this.session = session;
    }

    [HttpGet]
    [Route("/admin/group/member/{groupId:int}")]
    public IActionResult Index(int groupId)
    {
        GroupInfo group = this.session.Get<GroupInfo>(groupId);
        if (group == null)
        {
            return Content("指定的群组不存在！");
        }

        GroupMemberIndexModel model = new GroupMemberIndexModel();
        model.GroupId = groupId;
        model.Group = group;

        return View("GroupMemberIndex", model);
    }

    [HttpPost]
    [Route("/admin/group/member/list")]
    public IActionResult List(GroupMemberListQueryParam queryParam)
    {
        int totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<GroupMemberInfo> groupMemberList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<GroupMemberInfo>();

        GroupMemberListModel model = new GroupMemberListModel(listPager);
        model.GroupMemberList = groupMemberList;

        return View("GroupMemberList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(GroupMemberListQueryParam queryParam)
    {
        ICriteria criteria = this.session.CreateCriteria<GroupMemberInfo>();

        criteria = criteria.SetReadOnly(true);

        int groupId = queryParam.GroupId;
        criteria = criteria.Add(Restrictions.Where<GroupMemberInfo>(e => e.Group.Id == groupId));

        if (string.IsNullOrWhiteSpace(queryParam.LoginNameQuery) == false)
        {
            string loginNameQuery = queryParam.LoginNameQuery.Trim();
            criteria = criteria.Add(Restrictions.On<GroupMemberInfo>(e => e.LoginName).IsInsensitiveLike(loginNameQuery, MatchMode.Anywhere));
        }

        if (string.IsNullOrWhiteSpace(queryParam.UserNameQuery) == false)
        {
            string userNameQuery = queryParam.UserNameQuery.Trim();
            criteria = criteria.Add(Restrictions.On<GroupMemberInfo>(e => e.UserName).IsInsensitiveLike(userNameQuery, MatchMode.Anywhere));
        }

        return criteria;
    }

    [HttpPost]
    [Route("/admin/group/member/add/{groupId:int}")]
    public IActionResult Add(int groupId)
    {
        this.session.DefaultReadOnly = true;

        GroupInfo group = this.session.Get<GroupInfo>(groupId);

        if (group == null)
        {
            throw new ValidationException("指定的数据项不存在！");
        }

        GroupMemberAddModel model = new GroupMemberAddModel();
        model.GroupId = groupId;
        model.Group = group;

        return View("GroupMemberAdd", model);
    }

    [HttpPost]
    [Route("/admin/group/member/add/save")]
    [Transactional]
    public IActionResult AddSave(GroupMemberAddModel model)
    {
        if (!this.ModelState.IsValid)
        {
            throw new ValidationException(ValidationUtil.GetErrorMessages(this.ModelState));
        }

        int groupId = model.GroupId;
        GroupInfo group = this.session.Get<GroupInfo>(groupId);

        if (group == null)
        {
            throw new ValidationException("指定的群组不存在！");
        }

        string loginName = model.LoginName;
        UserInfo? user = this.session.Query<UserInfo>()
            .Where(e => e.LoginName != UserInfo.DevLoginName && e.LoginName == loginName)
            .Take(1)
            .SingleOrDefault();

        if (user == null)
        {
            throw new ValidationException("指定的用户不存在！");
        }

        if (user.Status == UserStatus.Frozen)
        {
            throw new ValidationException("指定的用户已冻结！");
        }

        // 检查是不是已经是群组的成员
        int userId = user.Id;
        int groupMemberIdExist = this.session.Query<GroupMemberInfo>()
            .Where(e => e.Group.Id == groupId && e.User.Id == userId)
            .Select(e => e.Id)
            .Take(1)
            .SingleOrDefault();
        if (groupMemberIdExist != 0)
        {
            throw new ValidationException("指定的用户已经是该群组的成员！");
        }

        GroupMemberInfo groupMember = new GroupMemberInfo();
        groupMember.Group = session.Load<GroupInfo>(groupId);
        groupMember.User = user;
        groupMember.LoginName = user.LoginName;
        groupMember.UserName = user.UserName;

        this.session.Save(groupMember);
        this.session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "保存成功！" }
        });
    }

    [HttpPost]
    [Route("/admin/group/member/remove")]
    [Transactional]
    public IActionResult Remove(int[] idList)
    {
        foreach (int id in idList)
        {
            GroupMemberInfo groupMember = this.session.Load<GroupMemberInfo>(id);
            this.session.Delete(groupMember);
        }

        this.session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "删除成功！" },
        });
    }
}