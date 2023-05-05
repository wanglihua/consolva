using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Order.Models.OrderPayType;
using shunda.Client.Services;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Coupon;
using shunda.Domain.LogisticsOrder;
using shunda.Service.Coupon;
using shunda.Service.Customer;
using shunda.Service.Fee;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Order.Controllers;

[OrderArea]
public class OrderPayTypeController : Controller {
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;
    private readonly LogisticsOrderPaySumService logisticsOrderPaySumService;
    private readonly CouponService couponService;
    private readonly LogisticsOrderFeeSumService logisticsOrderFeeSumService;
    private readonly CustomerScoreDeductService customerScoreDeductService;

    public OrderPayTypeController(
        ISession session,
        LoginCustomerService loginCustomerService,
        LogisticsOrderPaySumService logisticsOrderPaySumService,
        CouponService couponService,
        LogisticsOrderFeeSumService logisticsOrderFeeSumService,
        CustomerScoreDeductService customerScoreDeductService
    ) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
        this.logisticsOrderPaySumService = logisticsOrderPaySumService;
        this.couponService = couponService;
        this.logisticsOrderFeeSumService = logisticsOrderFeeSumService;
        this.customerScoreDeductService = customerScoreDeductService;
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
        var loginCustomer = loginCustomerService.GetLoginCustomer();

        if (order.Customer.Id != loginCustomerId) {
            throw new ValidationException($"指定的订单{order.Code}不属于当前登录客户#{loginCustomerId}！");
        }

        logisticsOrderPaySumService.SumFeeAndUpdatePayStatus(orderId);

        var orderFeeSumData = logisticsOrderFeeSumService.GetSumData(orderId);

        decimal couponDiscountAmount = 0;
        string couponDiscountAmountStr = string.Empty;
        if (coupon != null) {
            var discountResult = couponService.CalculateCouponDiscount(couponId, orderFeeSumData.FirstModeFeeCouponType);
            if (discountResult.Success == false) {
                couponDiscountAmount = 0;
                couponDiscountAmountStr = discountResult.Message;
            } else {
                couponDiscountAmount = discountResult.Discount;
                couponDiscountAmountStr = couponDiscountAmount.ToString("#0.00");
            }
        }

        decimal paidAmount = order.PaidAmount;
        decimal arrearsAmount = orderFeeSumData.TotalAmountWithoutCouponDiscount - couponDiscountAmount - paidAmount;
        if (arrearsAmount < 0) {
            arrearsAmount = 0;
        }

        var scoreDeductResult = customerScoreDeductService.GetDeduction(orderId, Math.Min(arrearsAmount, orderFeeSumData.FirstModeFeeCouponType));

        if (param.ScoreDeduct) {
            arrearsAmount -= scoreDeductResult.Amount;
        }

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new OrderPayTypeListModel {
                OrderId = order.Id,
                OrderCode = order.Code,
                TotalAmount = orderFeeSumData.TotalAmountWithoutDiscount.ToString("#0.00"),
                VipDiscountAmount = orderFeeSumData.VipDiscountAmount.ToString("#0.00"),
                CustomerScore = loginCustomer.ValidScore.ToString("#0.00"),
                ScoreDeductScore = scoreDeductResult.Score.ToString("#0.00"),
                ScoreDeductAmount = scoreDeductResult.Amount.ToString("#0.00"),
                HasCoupon = coupon != null,
                CouponDiscountAmount = couponDiscountAmountStr,
                PaidAmount = paidAmount.ToString("#0.00"),
                ArrearsAmount = arrearsAmount.ToString("#0.00"),
            }
        });
    }
}