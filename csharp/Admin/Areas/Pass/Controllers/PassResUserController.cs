using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using shunda.Admin.Areas.Pass.Models.PassResUser;
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
public class PassResUserController : Controller
{
    private static readonly string userAlias = "u";
    private readonly PassResService passResService;
    private readonly ISession session;

    public PassResUserController(ISession session, PassResService passResService)
    {
        this.session = session;
        this.passResService = passResService;
    }

    [HttpGet]
    [Route("/admin/pass/res/user/{passResId:int}")]
    public IActionResult Index(int passResId)
    {
        var passRes = session.Get<PassResInfo>(passResId);
        if (passRes == null) return Content("指定的资源不存在！");

        var model = new PassResUserIndexModel();
        model.PassResId = passResId;
        model.PassRes = passRes;

        return View("PassResUserIndex", model);
    }

    [HttpPost]
    [Route("/admin/pass/res/user/list")]
    public IActionResult List(PassResUserListQueryParam queryParam)
    {
        var totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        var listPager = new ListPager(queryParam, totalCount);

        var userPassResList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<UserPassResInfo>();

        var model = new PassResUserListModel(listPager);
        model.UserPassResList = userPassResList;
        model.UserAlias = userAlias;

        return View("PassResUserList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(PassResUserListQueryParam queryParam)
    {
        var passResId = queryParam.PassResId;

        var criteria = session.CreateCriteria<UserPassResInfo>();

        criteria = criteria.CreateAlias(nameof(UserPassResInfo.User), userAlias);

        criteria = criteria.Add(Restrictions.Where<UserPassResInfo>(e => e.PassRes.Id == passResId));

        criteria = criteria.SetReadOnly(true);

        return criteria;
    }

    [HttpPost]
    [Route("/admin/pass/res/user/add/{passResId:int}")]
    public IActionResult Add(int passResId)
    {
        var passRes = session.Get<PassResInfo>(passResId);
        if (passRes == null) throw new ValidationException("指定的资源不存在！");

        var model = new PassResUserAddModel();
        model.PassResId = passResId;
        model.PassRes = passRes;

        return View("PassResUserAdd", model);
    }

    [HttpPost]
    [Route("/admin/pass/res/user/add/save")]
    [Transactional]
    public IActionResult AddSave(PassResUserAddModel model)
    {
        if (!ModelState.IsValid) throw new ValidationException(ValidationUtil.GetErrorMessages(ModelState));

        var passResId = model.PassResId;
        var passRes = session.Get<PassResInfo>(passResId);
        if (passRes == null) throw new ValidationException("指定的资源不存在！");

        var loginName = model.LoginName;
        var user = session.Query<UserInfo>()
            .Where(e => e.LoginName != UserInfo.DevLoginName && e.LoginName == loginName)
            .Take(1)
            .SingleOrDefault();

        if (user == null) throw new ValidationException("指定的用户不存在！");

        if (user.Status == UserStatus.Frozen) throw new ValidationException("指定的用户已冻结！");

        var userId = user.Id;

        var userPassResInDb = session.Query<UserPassResInfo>()
            .Where(e => e.PassRes.Id == passResId && e.User.Id == userId)
            .Take(1)
            .SingleOrDefault();
        if (userPassResInDb != null) throw new ValidationException("该资源已经授权给指定的用户！");

        var userPassRes = new UserPassResInfo();
        userPassRes.User = session.Load<UserInfo>(userId);
        userPassRes.PassRes = session.Load<PassResInfo>(passResId);

        session.Save(userPassRes);
        session.Flush();

        passResService.CachePassResIdMapForAllUser();

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "message", "保存成功！" }
        });
    }

    [HttpPost]
    [Route("/admin/pass/res/user/remove")]
    [Transactional]
    public IActionResult Remove(int[] idList)
    {
        foreach (var id in idList)
        {
            var userPassRes = session.Load<UserPassResInfo>(id);
            session.Delete(userPassRes);
        }

        session.Flush();

        passResService.CachePassResIdMapForAllUser();

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "message", "删除成功！" }
        });
    }
}