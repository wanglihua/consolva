using Microsoft.AspNetCore.Mvc;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Order.Controllers;

[OrderArea]
public class OrderDetailController : Controller {
    private readonly ISession session;

    public OrderDetailController(ISession session) {
        this.session = session;
    }
}