using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NLog;
using shunda.Admin.Areas.Pass.Models.Admin;
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
public class AdminController : Controller
{
    private static Logger log = LogManager.GetCurrentClassLogger();

    private ISession session;
    private AdminUserService adminUserService;

    public AdminController(ISession session, AdminUserService adminUserService)
    {
        this.session = session;
        this.adminUserService = adminUserService;
    }

    [HttpGet]
    [Route("/admin/admin")]
    public IActionResult Index()
    {
        AdminIndexModel model = new AdminIndexModel();
        return View("AdminIndex", model);
    }

    [HttpPost]
    [Route("/admin/admin/list")]
    public IActionResult List(AdminListQueryParam queryParam)
    {
        int totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<AdminInfo> adminList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<AdminInfo>();

        AdminListModel model = new AdminListModel(listPager) {
            AdminList = adminList
        };

        return View("AdminList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(AdminListQueryParam queryParam)
    {
        string loginNameQuery = queryParam.LoginNameQuery ?? string.Empty;
        string userNameQuery = queryParam.UserNameQuery ?? string.Empty;

        ICriteria criteria = this.session.CreateCriteria<AdminInfo>();

        criteria = criteria.SetReadOnly(true);

        criteria = criteria.CreateAlias(nameof(AdminInfo.User), nameof(AdminInfo.User));

        string devLoginName = UserInfo.DevLoginName;
        criteria = criteria.Add(Restrictions.Not(Restrictions.Eq($"{nameof(AdminInfo.User)}.{nameof(AdminInfo.User.LoginName)}", devLoginName)));

        if (string.IsNullOrWhiteSpace(loginNameQuery) == false)
        {
            loginNameQuery = loginNameQuery.Trim();
            criteria = criteria.Add(Restrictions.InsensitiveLike($"{nameof(AdminInfo.User)}.{nameof(UserInfo.LoginName)}", loginNameQuery, MatchMode.Anywhere));
        }

        if (string.IsNullOrWhiteSpace(userNameQuery) == false)
        {
            userNameQuery = userNameQuery.Trim();
            criteria = criteria.Add(Restrictions.InsensitiveLike($"{nameof(AdminInfo.User)}.{nameof(UserInfo.UserName)}", userNameQuery, MatchMode.Anywhere));
        }

        return criteria;
    }

    [HttpPost]
    [Route("/admin/admin/add")]
    public IActionResult Add()
    {
        this.session.DefaultReadOnly = true;

        AdminAddModel model = new AdminAddModel();

        return View("AdminAdd", model);
    }

    [HttpPost]
    [Route("/admin/admin/save")]
    [Transactional]
    public IActionResult Save(AdminAddModel model)
    {
        if (!this.ModelState.IsValid)
        {
            throw new ValidationException(ValidationUtil.GetErrorMessages(this.ModelState));
        }

        string loginName = model.LoginName.Trim();

        // 检查当前登录名是否已经是管理员
        int loginNameAdminCount = this.session.Query<AdminInfo>().Where(e => e.User.LoginName == loginName).Count();
        if (loginNameAdminCount != 0)
        {
            throw new ValidationException("用户当前已经是管理员！");
        }

        // 保存为管理员
        UserInfo? user = this.session.Query<UserInfo>().Where(e => e.LoginName != UserInfo.DevLoginName && e.LoginName == loginName).Take(1).SingleOrDefault();
        if (user == null)
        {
            throw new ValidationException("指定的用户不存在！");
        }

        if (user.Status == UserStatus.Frozen)
        {
            throw new ValidationException("指定的用户已冻结！");
        }

        AdminInfo admin = new AdminInfo {
            User = user,
        };

        this.session.Save(admin);
        this.session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "管理员添加成功！" }
        });
    }

    [HttpPost]
    [Route("/admin/admin/remove")]
    [Transactional]
    public IActionResult Remove(int[] idList)
    {
        string supperAdminLoginName = AdminInfo.SupperAdminLoginName;

        int supperAdminCount = this.session.Query<AdminInfo>().Where(e => idList.Contains(e.Id) && e.User.LoginName == supperAdminLoginName).Count();

        if (supperAdminCount != 0)
        {
            throw new ValidationException("内置管理员 admin 不能移除！");
        }

        foreach (int id in idList)
        {
            AdminInfo admin = this.session.Load<AdminInfo>(id);
            session.Delete(admin);
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "删除成功！" },
        });
    }

    [HttpPost]
    [Route("/admin/admin/refresh/cache")]
    public IActionResult RefreshCache()
    {
        adminUserService.CacheAdminUserIdMap();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "管理员资源授权数据缓存刷新成功！" }
        });
    }
}