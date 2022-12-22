using Microsoft.AspNetCore.Mvc;
using shunda.Client.Services;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Coupon.Controllers;

[CouponArea]
public class CouponPayController : Controller {
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;

    public CouponPayController(ISession session, LoginCustomerService loginCustomerService) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
    }
}