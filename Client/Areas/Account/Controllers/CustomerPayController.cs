using Microsoft.AspNetCore.Mvc;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Account.Controllers;

[AccountArea]
public class CustomerPayController : Controller
{
    private readonly ISession session;

    public CustomerPayController(ISession session)
    {
        this.session = session;
    }
}