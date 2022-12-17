using Microsoft.AspNetCore.Mvc;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Coupon.Controllers;

[CouponArea]
public class CouponMineController : Controller {
    private readonly ISession session;

    public CouponMineController(ISession session) {
        this.session = session;
    }
}