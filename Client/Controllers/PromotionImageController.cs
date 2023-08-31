using Microsoft.AspNetCore.Mvc;
using shunda.Core.Web;
using ISession = NHibernate.ISession;

namespace shunda.Client.Controllers;

[NoLoginRequired]
public class PromotionImageController : Controller {
    private readonly ISession session;

    public PromotionImageController(ISession session) {
        this.session = session;
    }
}