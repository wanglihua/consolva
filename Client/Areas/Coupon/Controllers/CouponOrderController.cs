using Microsoft.AspNetCore.Mvc;
using shunda.Client.Services;
using shunda.Service.Coupon;
using shunda.Service.LogisticsOrder;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Coupon.Controllers;

[CouponArea]
public class CouponOrderController : Controller {
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;
    private readonly CouponBatchService couponBatchService;
    private readonly CouponService couponService;
    private readonly LogisticsOrderCreateSelectService logisticsOrderCreateSelectService;

    public CouponOrderController(
        ISession session,
        LoginCustomerService loginCustomerService,
        CouponBatchService couponBatchService,
        CouponService couponService,
        LogisticsOrderCreateSelectService logisticsOrderCreateSelectService
    ) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
        this.couponBatchService = couponBatchService;
        this.couponService = couponService;
        this.logisticsOrderCreateSelectService = logisticsOrderCreateSelectService;
    }
}