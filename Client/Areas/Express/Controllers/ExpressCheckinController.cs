using Microsoft.AspNetCore.Mvc;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Express.Controllers;

[ExpressArea]
public class ExpressCheckinController : Controller
{
    private ISession session;

    public ExpressCheckinController(ISession session)
    {
        this.session = session;
    }
}