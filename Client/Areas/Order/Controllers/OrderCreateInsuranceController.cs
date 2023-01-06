using Microsoft.AspNetCore.Mvc;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Order.Controllers;

[OrderArea]
public class OrderCreateInsuranceController : Controller {
    private readonly ISession session;

    public OrderCreateInsuranceController(ISession session) {
        this.session = session;
    }
}