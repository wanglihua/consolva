using Microsoft.AspNetCore.Mvc;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Account.Controllers;

[AccountArea]
public class CustomerScoreLogController : Controller
{
    private readonly ISession session;

    public CustomerScoreLogController(ISession session)
    {
        this.session = session;
    }
}