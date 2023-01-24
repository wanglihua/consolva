using Microsoft.AspNetCore.Mvc;
using shunda.Admin.Services;
using shunda.Core.Web;
using shunda.Domain.Pass.PassRes;
using shunda.Service.Express;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Express.Controllers;

[ExpressArea]
[AdminAuth(PassRes.Express.ExpressList)]
public class ExpressRemarkController : Controller {
    private readonly ISession session;
    private readonly LoginUserService loginUserService;
    private readonly ExpressRemarkService expressRemarkService;

    public ExpressRemarkController(ISession session,
        LoginUserService loginUserService,
        ExpressRemarkService expressRemarkService
    ) {
        this.session = session;
        this.loginUserService = loginUserService;
        this.expressRemarkService = expressRemarkService;
    }
    
    
}