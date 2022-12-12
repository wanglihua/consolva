using Microsoft.AspNetCore.Mvc;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Account.Controllers;

public class CustomerAccountEntryController : Controller
{
    private readonly ISession session;

    public CustomerAccountEntryController(ISession session)
    {
        this.session = session;
    }
}