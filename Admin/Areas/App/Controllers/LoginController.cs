using Microsoft.AspNetCore.Mvc;
using shunda.Core.Web;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.App.Controllers;

[AppArea]
[NoLoginRequired]
public class LoginController : Controller {
    private readonly ISession session;

    public LoginController(ISession session) {
        this.session = session;
    }
}