using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using shunda.Admin.Areas.Pass.Models.UserLoginLog;
using shunda.Core;
using shunda.Core.Web;
using shunda.Domain.Pass;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Pass.Controllers;

[AdminAuth(AdminOnly = true)]
[PassArea]
public class UserLoginLogController : Controller
{
    private readonly ISession session;

    public UserLoginLogController(ISession session)
    {
        this.session = session;
    }

    [HttpGet]
    [Route("/admin/user/login/log")]
    public IActionResult Index()
    {
        var model = new UserLoginLogIndexModel();
        return View("UserLoginLogIndex", model);
    }

    [HttpPost]
    [Route("/admin/user/login/log/list")]
    public IActionResult List(UserLoginLogListQueryParam queryParam)
    {
        var totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        var listPager = new ListPager(queryParam, totalCount);

        var userLoginLogList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<UserLoginLogInfo>();

        foreach (var userLoginLogInfo in userLoginLogList)
        {
            // userLoginLogInfo
        }

        var model = new UserLoginLogListModel(listPager);
        model.UserLoginLogList = userLoginLogList;

        return View("UserLoginLogList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(UserLoginLogListQueryParam queryParam)
    {
        var criteria = session.CreateCriteria<UserLoginLogInfo>();
        criteria = criteria.SetReadOnly(true);

        criteria = criteria.CreateAlias(nameof(UserLoginLogInfo.User), nameof(UserLoginLogInfo.User));

        var userQuery = queryParam.UserQuery ?? string.Empty;
        if (string.IsNullOrWhiteSpace(userQuery) == false)
        {
            userQuery = userQuery.Trim();
            criteria = criteria.Add(
                Restrictions.Or(
                    Restrictions.Like($"{nameof(UserLoginLogInfo.User)}.{nameof(UserLoginLogInfo.User.LoginName)}", userQuery, MatchMode.Anywhere),
                    Restrictions.Like($"{nameof(UserLoginLogInfo.User)}.{nameof(UserLoginLogInfo.User.UserName)}", userQuery, MatchMode.Anywhere)
                )
            );
        }

        return criteria;
    }
}