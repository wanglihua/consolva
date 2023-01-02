using Microsoft.AspNetCore.Mvc;
using shunda.Core.Web;
using shunda.Domain.Pass.PassRes;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Express.Controllers;

[ExpressArea]
[AdminAuth(PassRes.Express.ExpressReturn)]
public class ExpressReturnController : Controller {
    private readonly ISession session;

    public ExpressReturnController(ISession session) {
        this.session = session;
    }
}