using Microsoft.AspNetCore.Mvc;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Order.Controllers;

[OrderArea]
public class OrderCreateAddressReceiveController : Controller {
    private readonly ISession session;

    public OrderCreateAddressReceiveController(ISession session) {
        this.session = session;
    }
}