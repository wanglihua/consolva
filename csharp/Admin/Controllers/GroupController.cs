using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using shunda.Admin.Models.Group;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Pass;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Controllers;

[AdminAuth(AdminOnly = true)]
public class GroupController : Controller
{
    private ISession session;

    public GroupController(ISession session)
    {
        this.session = session;
    }

    [HttpGet]
    [Route("/admin/group")]
    public IActionResult Index()
    {
        GroupIndexModel model = new GroupIndexModel();
        return View("GroupIndex", model);
    }

    [HttpPost]
    [Route("/admin/group/list")]
    public IActionResult List(GroupListQueryParam queryParam)
    {
        int totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<GroupInfo> groupList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<GroupInfo>();

        GroupListModel model = new GroupListModel(listPager);
        model.GroupList = groupList;

        return View("GroupList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(GroupListQueryParam queryParam)
    {
        ICriteria criteria = this.session.CreateCriteria<GroupInfo>();
        criteria = criteria.SetReadOnly(true);
        
        if (string.IsNullOrWhiteSpace(queryParam.CodeQuery) == false)
        {
            string codeQuery = queryParam.CodeQuery.Trim();
            criteria = criteria.Add(Restrictions.On<GroupInfo>(e => e.Code).IsInsensitiveLike(codeQuery, MatchMode.Anywhere));
        }
        
        if (string.IsNullOrWhiteSpace(queryParam.NameQuery) == false)
        {
            string nameQuery = queryParam.NameQuery.Trim();
            criteria = criteria.Add(Restrictions.On<GroupInfo>(e => e.Name).IsInsensitiveLike(nameQuery, MatchMode.Anywhere));
        }

        if (string.IsNullOrWhiteSpace(queryParam.UserQuery) == false)
        {
            string userQuery = queryParam.UserQuery;

            DetachedCriteria groupIdCriteria = DetachedCriteria.For<GroupMemberInfo>();
            groupIdCriteria = groupIdCriteria.CreateAlias(nameof(GroupMemberInfo.User), nameof(GroupMemberInfo.User));

            groupIdCriteria = groupIdCriteria.Add(Restrictions.Or(
                Restrictions.Like($"{nameof(GroupMemberInfo.User)}.{nameof(GroupMemberInfo.User.LoginName)}", userQuery, MatchMode.Anywhere),
                Restrictions.Like($"{nameof(GroupMemberInfo.User)}.{nameof(GroupMemberInfo.User.UserName)}", userQuery, MatchMode.Anywhere)
            ));

            groupIdCriteria.SetProjection(Projections.Property($"{nameof(GroupMemberInfo.Group)}.{nameof(GroupMemberInfo.Group.Id)}"));

            criteria = criteria.Add(Subqueries.PropertyIn(nameof(GroupInfo.Id), groupIdCriteria));
        }

        return criteria;
    }

    [HttpPost]
    [Route("/admin/group/detail/{id:int?}")]
    public IActionResult Detail(int? id)
    {
        this.session.DefaultReadOnly = true;

        GroupInfo group;

        if (id == null || id.Value == 0)
        {
            group = new GroupInfo();
        }
        else
        {
            group = this.session.Get<GroupInfo>(id.Value);
            if (group == null)
            {
                throw new ValidationException("指定的数据项不存在！");
            }
        }

        GroupDetailModel model = new GroupDetailModel();
        model.GroupId = group.Id;
        model.Code = group.Code;
        model.Name = group.Name;
        model.Description = group.Description;

        return View("GroupDetail", model);
    }

    [HttpPost]
    [Route("/admin/group/save")]
    [Transactional]
    public IActionResult Save(GroupDetailModel model)
    {
        if (!this.ModelState.IsValid)
        {
            throw new ValidationException(ValidationUtil.GetErrorMessages(this.ModelState));
        }

        int groupId = model.GroupId;

        bool isCreate = groupId == 0;

        GroupInfo group;

        if (isCreate)
        {
            group = new GroupInfo();
        }
        else
        {
            group = this.session.Get<GroupInfo>(groupId);
        }

        group.Code = model.Code;
        group.Name = model.Name;
        group.Description = model.Description;

        if (isCreate)
        {
            this.session.Save(group);
        }
        else
        {
            this.session.Update(group);
        }

        this.session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "保存成功！" }
        });
    }

    [HttpPost]
    [Route("/admin/group/delete")]
    [Transactional]
    public IActionResult Delete(int[] idList)
    {
        foreach (int id in idList)
        {
            GroupInfo group = this.session.Load<GroupInfo>(id);
            this.session.Delete(group);
        }

        this.session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "删除成功！" },
        });
    }
}