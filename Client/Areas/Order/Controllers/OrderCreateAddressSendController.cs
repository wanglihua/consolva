using Microsoft.AspNetCore.Mvc;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Order.Controllers;

[OrderArea]
public class OrderCreateAddressSendController : Controller {
    private readonly ISession session;

    public OrderCreateAddressSendController(ISession session) {
        this.session = session;
    }
}