using Microsoft.AspNetCore.Mvc;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Customer.Controllers;

[CustomerArea]
public class CustomerReceiveAddressController : Controller
{
    private readonly ISession session;

    public CustomerReceiveAddressController(ISession session)
    {
        this.session = session;
    }
}