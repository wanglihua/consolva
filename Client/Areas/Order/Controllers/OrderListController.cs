using Microsoft.AspNetCore.Mvc;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Order.Controllers;

[OrderArea]
public class OrderListController : Controller {
    private readonly ISession session;

    public OrderListController(ISession session) {
        this.session = session;
    }
}