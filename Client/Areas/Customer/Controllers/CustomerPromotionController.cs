using Microsoft.AspNetCore.Mvc;
using NLog;
using shunda.Client.Services;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Customer.Controllers;

[CustomerArea]
public class CustomerPromotionController : Controller {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;

    public CustomerPromotionController(
        ISession session,
        LoginCustomerService loginCustomerService
    ) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
    }
}