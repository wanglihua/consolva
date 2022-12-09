using Microsoft.AspNetCore.Mvc;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Express.Controllers;

public class ExpressClaimController : Controller
{
    private readonly ISession session;
    public ExpressClaimController(ISession session)
    {
        this.session = session;
    }
    
    
}