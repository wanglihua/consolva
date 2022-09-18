using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using shunda.Admin.Models.PassResUser;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Pass;
using shunda.Service.Pass;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Controllers;

[AdminAuth(AdminOnly = true)]
public class PassResUserController : Controller
{
    private ISession session;
    private PassResService passResService;

    public PassResUserController(ISession session, PassResService passResService)
    {
        this.session = session;
        this.passResService = passResService;
    }

    [HttpGet]
    [Route("/admin/pass/res/user/{passResId:int}")]
    public IActionResult Index(int passResId)
    {
        PassResInfo passRes = this.session.Get<PassResInfo>(passResId);
        if (passRes == null)
        {
            return Content("指定的资源不存在！");
        }

        PassResUserIndexModel model = new PassResUserIndexModel();
        model.PassResId = passResId;
        model.PassRes = passRes;

        return View("PassResUserIndex", model);
    }

    [HttpPost]
    [Route("/admin/pass/res/user/list")]
    public IActionResult List(PassResUserListQueryParam queryParam)
    {
        int totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<UserPassResInfo> userPassResList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<UserPassResInfo>();

        PassResUserListModel model = new PassResUserListModel(listPager);
        model.UserPassResList = userPassResList;
        model.UserAlias = userAlias;

        return View("PassResUserList", model);
    }

    private static readonly string userAlias = "u";

    [NonAction]
    private ICriteria GetListCriteria(PassResUserListQueryParam queryParam)
    {
        int passResId = queryParam.PassResId;

        ICriteria criteria = this.session.CreateCriteria<UserPassResInfo>();

        criteria = criteria.CreateAlias(nameof(UserPassResInfo.User), userAlias);

        criteria = criteria.Add(Restrictions.Where<UserPassResInfo>(e => e.PassRes.Id == passResId));

        criteria = criteria.SetReadOnly(true);

        return criteria;
    }

    [HttpPost]
    [Route("/admin/pass/res/user/add/{passResId:int}")]
    public IActionResult Add(int passResId)
    {
        this.session.DefaultReadOnly = true;

        PassResInfo passRes = this.session.Get<PassResInfo>(passResId);
        if (passRes == null)
        {
            throw new ValidationException("指定的资源不存在！");
        }

        PassResUserAddModel model = new PassResUserAddModel();
        model.PassResId = passResId;
        model.PassRes = passRes;

        return View("PassResUserAdd", model);
    }

    [HttpPost]
    [Route("/admin/pass/res/user/add/save")]
    [Transactional]
    public IActionResult AddSave(PassResUserAddModel model)
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

        int userId = user.Id;

        var userPassResInDb = this.session.Query<UserPassResInfo>()
            .Where(e => e.PassRes.Id == passResId && e.User.Id == userId)
            .Take(1)
            .SingleOrDefault();
        if (userPassResInDb != null)
        {
            throw new ValidationException("该资源已经授权给指定的用户！");
        }

        UserPassResInfo userPassRes = new UserPassResInfo();
        userPassRes.User = this.session.Load<UserInfo>(userId);
        userPassRes.PassRes = this.session.Load<PassResInfo>(passResId);

        this.session.Save(userPassRes);
        this.session.Flush();

        passResService.CachePassResIdMapForAllUser();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "保存成功！" }
        });
    }

    [HttpPost]
    [Route("/admin/pass/res/user/remove")]
    [Transactional]
    public IActionResult Remove(int[] idList)
    {
        foreach (int id in idList)
        {
            UserPassResInfo userPassRes = this.session.Load<UserPassResInfo>(id);
            this.session.Delete(userPassRes);
        }

        this.session.Flush();

        passResService.CachePassResIdMapForAllUser();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "删除成功！" },
        });
    }
}