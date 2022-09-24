using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using shunda.Admin.Areas.Pass.Models.UserLoginLog;
using shunda.Core;
using shunda.Core.Web;
using shunda.Domain.Pass;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Pass.Controllers;

[AdminAuth]
[PassArea]
public class UserLoginLogController : Controller
{
    private ISession session;

    public UserLoginLogController(ISession session)
    {
        this.session = session;
    }

    [HttpGet]
    [Route("/admin/user/login/log")]
    public IActionResult Index()
    {
        UserLoginLogIndexModel model = new UserLoginLogIndexModel();
        return View("UserLoginLogIndex", model);
    }

    [HttpPost]
    [Route("/admin/user/login/log/list")]
    public IActionResult List(UserLoginLogListQueryParam queryParam)
    {
        int totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<UserLoginLogInfo> userLoginLogList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<UserLoginLogInfo>();

        foreach (UserLoginLogInfo userLoginLogInfo in userLoginLogList)
        {
            // userLoginLogInfo
        }

        UserLoginLogListModel model = new UserLoginLogListModel(listPager);
        model.UserLoginLogList = userLoginLogList;

        return View("UserLoginLogList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(UserLoginLogListQueryParam queryParam)
    {
        ICriteria criteria = session.CreateCriteria<UserLoginLogInfo>();
        criteria = criteria.SetReadOnly(true);

        criteria = criteria.CreateAlias(nameof(UserLoginLogInfo.User), nameof(UserLoginLogInfo.User));

        string userQuery = queryParam.UserQuery ?? string.Empty;
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