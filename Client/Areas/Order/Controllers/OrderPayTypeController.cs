using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Order.Models.OrderPayType;
using shunda.Client.Services;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Coupon;
using shunda.Domain.Fee;
using shunda.Domain.LogisticsOrder;
using shunda.Service.Coupon;
using shunda.Service.Fee;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Order.Controllers;

[OrderArea]
public class OrderPayTypeController : Controller {
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;
    private readonly LogisticsOrderPaySumService logisticsOrderPaySumService;
    private readonly CouponService couponService;

    public OrderPayTypeController(
        ISession session,
        LoginCustomerService loginCustomerService,
        LogisticsOrderPaySumService logisticsOrderPaySumService,
        CouponService couponService
    ) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
        this.logisticsOrderPaySumService = logisticsOrderPaySumService;
        this.couponService = couponService;
    }

    [HttpPost]
    [Route("/client/order/pay/type/list")]
    [Transactional]
    public IActionResult PayTypeList([FromBody] OrderPayTypeListParam param) {
        ValidationUtil.Validate<OrderPayTypeListParamValidator, OrderPayTypeListParam>(param);

        int orderId = param.OrderId;
        int couponId = param.CouponId;
        var order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null) {
            throw new ValidationException($"指定的订单#{orderId}不存在！");
        }

        CouponInfo? coupon = null;
        if (couponId > 0) {
            coupon = session.Get<CouponInfo>(couponId);
            if (coupon == null) {
                throw new ValidationException($"指定的优惠券{couponId}不存在！");
            }
        }

        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        if (order.Customer.Id != loginCustomerId) {
            throw new ValidationException($"指定的订单{order.Code}不属于当前登录客户#{loginCustomerId}！");
        }

        logisticsOrderPaySumService.SumFeeAndUpdatePayStatus(orderId);

        // decimal totalAmount = order.OrderAmount; 不能把优惠费用计算在内
        var orderFeeList = session.Query<LogisticsOrderFeeInfo>()
            .Where(e => e.Order.Id == orderId)
            .ToList();

        decimal totalAmount = 0.0m;
        foreach (var fee in orderFeeList) {
            if (fee.Type != LogisticsOrderFeeType.CouponDiscountFee) {
                totalAmount += fee.Fee;
            }
        }

        decimal discountAmount = 0;
        string discountAmountStr = string.Empty;
        if (coupon != null) {
            var discountResult = couponService.CalculateCouponDiscount(couponId, totalAmount);
            if (discountResult.Success == false) {
                discountAmount = 0;
                discountAmountStr = discountResult.Message;
            } else {
                discountAmount = discountResult.Discount;
                discountAmountStr = discountAmount.ToString("#0.00");
            }
        }

        decimal paidAmount = order.PaidAmount;
        decimal arrearsAmount = totalAmount - discountAmount - paidAmount;

        if (arrearsAmount < 0) {
            arrearsAmount = 0;
        }

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new OrderPayTypeListModel {
                OrderId = order.Id,
                OrderCode = order.Code,
                TotalAmount = totalAmount.ToString("#0.00"),
                HasCoupon = coupon != null,
                DiscountAmount = discountAmountStr,
                PaidAmount = paidAmount.ToString("#0.00"),
                ArrearsAmount = arrearsAmount.ToString("#0.00"),
            }
        });
    }
}