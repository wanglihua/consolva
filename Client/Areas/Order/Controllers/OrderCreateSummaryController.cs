using Microsoft.AspNetCore.Mvc;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Order.Controllers;

[OrderArea]
public class OrderCreateSummaryController : Controller {
    private readonly ISession session;

    public OrderCreateSummaryController(ISession session) {
        this.session = session;
    }
}