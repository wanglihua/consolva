using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Order.Models.OrderPayCoupon;
using shunda.Client.Services;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Service.Coupon;
using shunda.Service.Fee;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Order.Controllers;

[OrderArea]
public class OrderPayCouponController : Controller {
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;
    private readonly CouponService couponService;
    private readonly CouponBatchService couponBatchService;
    private readonly LogisticsOrderPaySumService logisticsOrderPaySumService;

    public OrderPayCouponController(ISession session,
        LoginCustomerService loginCustomerService,
        CouponService couponService,
        CouponBatchService couponBatchService,
        LogisticsOrderPaySumService logisticsOrderPaySumService
    ) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
        this.couponService = couponService;
        this.couponBatchService = couponBatchService;
        this.logisticsOrderPaySumService = logisticsOrderPaySumService;
    }

    [HttpPost]
    [Route("/client/order/pay/coupon/has")]
    public IActionResult Has([FromBody] OrderPayCouponHasParam param) {
        ValidationUtil.Validate<OrderPayCouponHasParamValidator, OrderPayCouponHasParam>(param);

        int loginCustomerId = loginCustomerService.GetLoginCustomerId();
        int orderId = param.OrderId;

        bool hasCoupon = couponService.HasCouponWhenPay(customerId: loginCustomerId, orderId: orderId);

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new OrderPayCouponHasModel {
                HasCoupon = hasCoupon,
            },
        });
    }

    [HttpPost]
    [Route("/client/order/pay/coupon/list")]
    public IActionResult List([FromBody] OrderPayCouponListParam param) {
        ValidationUtil.Validate<OrderPayCouponListParamValidator, OrderPayCouponListParam>(param);

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }
}