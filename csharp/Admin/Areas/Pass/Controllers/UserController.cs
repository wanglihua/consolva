using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using shunda.Admin.Areas.Pass.Models.User;
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
public class UserController : Controller
{
    private ISession session;
    private PasswordService passwordService;

    public UserController(ISession session, PasswordService passwordService)
    {
        this.session = session;
        this.passwordService = passwordService;
    }

    [HttpGet]
    [Route("/admin/user")]
    public IActionResult Index()
    {
        UserIndexModel model = new UserIndexModel();
        return View("UserIndex", model);
    }

    [HttpPost]
    [Route("/admin/user/list")]
    public IActionResult List(UserListQueryParam queryParam)
    {
        int totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        var listCriteria = GetListCriteria(queryParam);

        if (listPager.SortField != nameof(UserInfo.Status))
        {
            listCriteria = listCriteria
                .AddOrder(new Order(nameof(UserInfo.Status), true))
                .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
                .AddOrder(Order.Desc(nameof(Entity.Id)));
        }
        else
        {
            listCriteria = listCriteria.AddOrder(new Order(listPager.SortField, listPager.IsAscending))
                .AddOrder(Order.Desc(nameof(Entity.Id)));
        }

        IList<UserInfo> userList = listCriteria
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<UserInfo>();

        UserListModel model = new UserListModel(listPager);
        model.UserList = userList;

        return View("UserList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(UserListQueryParam queryParam)
    {
        ICriteria criteria = this.session.CreateCriteria<UserInfo>();
        criteria = criteria.SetReadOnly(true);

        string devLoginName = UserInfo.DevLoginName;
        criteria = criteria.Add(Restrictions.Where<UserInfo>(e => e.LoginName != devLoginName));

        if (string.IsNullOrWhiteSpace(queryParam.LoginNameQuery) == false)
        {
            string loginNameQuery = queryParam.LoginNameQuery.Trim();
            criteria = criteria.Add(Restrictions.On<UserInfo>(e => e.LoginName).IsInsensitiveLike(loginNameQuery, MatchMode.Anywhere));
        }

        if (string.IsNullOrWhiteSpace(queryParam.UserNameQuery) == false)
        {
            string userNameQuery = queryParam.UserNameQuery.Trim();
            criteria = criteria.Add(Restrictions.On<UserInfo>(e => e.UserName).IsInsensitiveLike(userNameQuery, MatchMode.Anywhere));
        }

        return criteria;
    }

    [HttpPost]
    [Route("/admin/user/detail/{id:int?}")]
    public IActionResult Detail(int? id)
    {
        UserInfo user;

        if (id == null || id.Value == 0)
        {
            user = new UserInfo();
        }
        else
        {
            user = this.session.Get<UserInfo>(id.Value);
            if (user == null)
            {
                throw new ValidationException("指定的数据项不存在！");
            }
        }

        UserDetailModel model = new UserDetailModel();
        model.UserId = user.Id;
        model.LoginName = user.LoginName;
        model.UserName = user.UserName;
        model.Password = string.Empty;

        return View("UserDetail", model);
    }

    [HttpPost]
    [Route("/admin/user/save")]
    [Transactional]
    public IActionResult Save(UserDetailModel model)
    {
        int userId = model.UserId;

        bool isCreate = userId == 0;

        if (isCreate && string.IsNullOrWhiteSpace(model.Password))
        {
            this.ModelState.AddModelError(nameof(UserInfo.Password), "密码不能为空！");
        }

        if (!this.ModelState.IsValid)
        {
            throw new ValidationException(ValidationUtil.GetErrorMessages(this.ModelState));
        }

        UserInfo user;

        if (isCreate)
        {
            user = new UserInfo();
            user.Status = UserStatus.Normal;
        }
        else
        {
            user = this.session.Get<UserInfo>(userId);
        }

        user.LoginName = model.LoginName;
        user.UserName = model.UserName;
        // user.LastLoginTime = DateTime.Now;

        if (string.IsNullOrWhiteSpace(model.Password) == false)
        {
            user.Password = this.passwordService.Encrypt(model.Password.Trim()); //略去密码的前后端的空格
        }

        if (isCreate)
        {
            session.Save(user);
        }
        else
        {
            session.Update(user);
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "保存成功！" }
        });
    }

    [HttpPost]
    [Route("/admin/user/delete")]
    [Transactional]
    public IActionResult Delete(int[] idList)
    {
        // throw new ValidationException("出错了，测试用的！");

        foreach (int id in idList)
        {
            UserInfo user = this.session.Load<UserInfo>(id);
            this.session.Delete(user);
        }

        this.session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "删除成功！" },
        });
    }

    [HttpPost]
    [Route("/admin/user/unfreeze")]
    [Transactional]
    public IActionResult Unfreeze(int id)
    {
        int userId = id;

        UserInfo userInfo = session.Get<UserInfo>(userId);
        if (userInfo == null)
        {
            throw new ValidationException("指定的用户不存在！");
        }

        if (userInfo.Status != UserStatus.Frozen)
        {
            throw new ValidationException("用户当前不是冻结状态！");
        }

        userInfo.Status = UserStatus.Normal;
        session.Update(userInfo);
        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "用户解冻成功！" },
        });
    }

    [HttpPost]
    [Route("/admin/user/freeze")]
    [Transactional]
    public IActionResult Freeze(int id)
    {
        int userId = id;

        UserInfo userInfo = session.Get<UserInfo>(userId);
        if (userInfo == null)
        {
            throw new ValidationException("指定的用户不存在！");
        }

        if (userInfo.Status == UserStatus.Frozen)
        {
            throw new ValidationException("用户已经是冻结状态！");
        }

        userInfo.Status = UserStatus.Frozen;
        session.Update(userInfo);
        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "用户冻结成功！" },
        });
    }
}