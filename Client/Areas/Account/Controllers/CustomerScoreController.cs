using Microsoft.AspNetCore.Mvc;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Account.Controllers;

[AccountArea]
public class CustomerScoreController : Controller
{
    private readonly ISession session;

    public CustomerScoreController(ISession session)
    {
        this.session = session;
    }
}