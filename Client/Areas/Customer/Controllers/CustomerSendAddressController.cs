using Microsoft.AspNetCore.Mvc;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Customer.Controllers;

[CustomerArea]
public class CustomerSendAddressController : Controller
{
    private readonly ISession session;

    public CustomerSendAddressController(ISession session)
    {
        this.session = session;
    }
}