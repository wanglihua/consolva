using Alipay.AopSdk.AspnetCore;
using Alipay.AopSdk.Core.Domain;
using Alipay.AopSdk.Core.Request;
using Newtonsoft.Json;
using NHibernate;
using NLog;
using shunda.Core;
using shunda.Core.Service;
using shunda.Core.Util;
using shunda.Domain.Coupon;
using shunda.Domain.Fee;
using shunda.Domain.LogisticsOrder;
using shunda.Service.Coupon;
using shunda.Service.Customer;
using shunda.Service.Fee;
using shunda.Service.LogisticsOrder;

namespace shunda.Service.Pay;

[Service(ServiceType.Scoped)]
public class AlipayOrderService {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;
    private readonly PayService payService;
    private readonly AlipayService alipayService;
    private readonly LogisticsOrderActionService logisticsOrderActionService;
    private readonly CustomerScoreService customerScoreService;
    private readonly LogisticsOrderPaySumService logisticsOrderPaySumService;
    private readonly CouponService couponService;
    private readonly LogisticsOrderFeeSumService logisticsOrderFeeSumService;
    private readonly CustomerScoreDeductService customerScoreDeductService;

    public AlipayOrderService(
        ISession session,
        PayService payService,
        AlipayService alipayService,
        LogisticsOrderActionService logisticsOrderActionService,
        CustomerScoreService customerScoreService,
        LogisticsOrderPaySumService logisticsOrderPaySumService,
        CouponService couponService,
        LogisticsOrderFeeSumService logisticsOrderFeeSumService,
        CustomerScoreDeductService customerScoreDeductService
    ) {
        this.session = session;
        this.payService = payService;
        this.alipayService = alipayService;
        this.logisticsOrderActionService = logisticsOrderActionService;
        this.customerScoreService = customerScoreService;
        this.logisticsOrderPaySumService = logisticsOrderPaySumService;
        this.couponService = couponService;
        this.logisticsOrderFeeSumService = logisticsOrderFeeSumService;
        this.customerScoreDeductService = customerScoreDeductService;
    }

    /// <summary>
    /// 创建支付宝支付返回 alipay request url
    /// </summary>
    /// <param name="param">参数对象</param>
    /// <returns>alipay request url 用于跳转支付宝支付</returns>
    public string Create(AlipayOrderCreateParam param) {
        ValidationUtil.Validate<AlipayOrderCreateParamValidator, AlipayOrderCreateParam>(param);

        log.Warn("支付宝支付发起 begin");

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

        if (order.ArrearsAmountFormula <= 0) {
            throw new ValidationException("当前订单无欠费金额，无需支付！");
        }

        var orderFeeSumData = logisticsOrderFeeSumService.GetSumData(orderId);

        decimal discountAmount = 0;
        if (coupon != null) {
            if (coupon.Customer.Id != order.Customer.Id) {
                throw new ValidationException("优惠券持有客户和订单客户不一致！");
            }

            var discountResult = couponService.CalculateCouponDiscount(couponId, orderFeeSumData.FirstModeFeeCouponType);
            if (discountResult.Success == false) {
                discountAmount = 0;
            } else {
                discountAmount = discountResult.Discount;

                // 保存优惠券到订单，并生成响应的订单费用，负的费用
                int customerId = order.Customer.Id;
                couponService.SaveToOrder(customerId: customerId, orderId: orderId, couponId: couponId);
            }
        }

        decimal paidAmount = order.PaidAmount;
        decimal arrearsAmount = orderFeeSumData.TotalAmountWithoutCouponDiscount - discountAmount - paidAmount;

        if (arrearsAmount < 0) {
            arrearsAmount = 0;
        }

        decimal scoreDeductBaseAmount = 0;
        if (param.ScoreDeduct) {
            scoreDeductBaseAmount = Math.Min(arrearsAmount, orderFeeSumData.FirstModeFeeCouponType);
            var scoreDeductResult = customerScoreDeductService.GetDeduction(orderId, scoreDeductBaseAmount);
            arrearsAmount -= scoreDeductResult.Amount;
        }

        // 外部订单号，商户网站订单系统中唯一的订单号
        string outTradeNo = payService.GenerateOutTradeNo();

        if (arrearsAmount <= 0) {
            // 如果欠费金额是0，就不用生成微信支付了，直接成功支付就可以了
            var orderPayZeroAmount = new LogisticsOrderPayInfo {
                Order = order,
                OrderCode = order.Code,
                Customer = order.Customer,
                PayType = PayType.AliPay,
                OutTradeNo = outTradeNo,
                Amount = 0, // 特殊情况的时候：订单用户原来支付过了一些，第二次支付的时候获得了优惠券折扣，这个折扣正好完全抵掉了欠款，这时就会生成一个支付金额为0的记录，同时把订单标记为已支付。
                ScoreDeductBaseAmount = scoreDeductBaseAmount,
                Status = PayStatus.Paid
            };

            session.Save(orderPayZeroAmount);
            session.Flush();

            logisticsOrderPaySumService.SumFeeAndUpdatePayStatus(orderId); // 这个这里可以不调用，因为 couponService.SaveToOrder()里面已经调用过一次了

            return string.Empty; // 无需支付返回空的url
        }

        // 付款金额
        string arrearsAmountStr = arrearsAmount.ToString("#0.00");

        LogisticsOrderPayInfo orderPay = new LogisticsOrderPayInfo {
            Order = order,
            OrderCode = order.Code,
            Customer = order.Customer,
            PayType = PayType.AliPay,
            OutTradeNo = outTradeNo,
            Amount = arrearsAmount,
            ScoreDeductBaseAmount = scoreDeductBaseAmount,
            Status = PayStatus.NotPaid
        };
        session.Save(orderPay);
        session.Flush();

        // 订单名称
        string subject = $"订单{order.Code}_{orderPay.Id}";

        // 商品描述
        string itemBody = $"订单{order.Code}_{orderPay.Id}";

        if (param.IsMobile == false) {
            // 组装业务参数model
            AlipayTradePagePayModel payModel = new AlipayTradePagePayModel {
                Subject = subject,
                Body = itemBody,
                TotalAmount = arrearsAmountStr,
                OutTradeNo = outTradeNo,
                ProductCode = "FAST_INSTANT_TRADE_PAY"
            };

            AlipayTradePagePayRequest payRequest = new AlipayTradePagePayRequest();
            // 设置同步回调地址
            payRequest.SetReturnUrl(param.ReturnUrl);
            // 设置异步通知接收地址
            payRequest.SetNotifyUrl(param.NotifyUrl);
            // 将业务model载入到request
            payRequest.SetBizModel(payModel);

            var response = alipayService.SdkExecute(payRequest);

            log.Warn("支付宝支付发起 end");

            string alipayRequestUrl = alipayService.Options.Gatewayurl + "?" + response.Body;

            //跳转支付宝支付
            return alipayRequestUrl;
        } else {
            // 组装业务参数model
            AlipayTradeWapPayModel payModel = new AlipayTradeWapPayModel {
                Subject = subject,
                Body = itemBody,
                TotalAmount = arrearsAmountStr,
                OutTradeNo = outTradeNo,
                ProductCode = "QUICK_WAP_WAY"
            };

            AlipayTradeWapPayRequest payRequest = new AlipayTradeWapPayRequest();
            // 设置同步回调地址
            payRequest.SetReturnUrl(param.ReturnUrl);
            // 设置异步通知接收地址
            payRequest.SetNotifyUrl(param.NotifyUrl);
            // 将业务model载入到request
            payRequest.SetBizModel(payModel);

            var response = alipayService.SdkExecute(payRequest);

            log.Warn("支付宝支付发起 end");

            string alipayRequestUrl = alipayService.Options.Gatewayurl + "?" + response.Body;

            //跳转支付宝支付
            return alipayRequestUrl;
        }
    }

    public void Callback(Dictionary<string, string> callbackDataDict) {
        try {
            log.Error("支付宝支付回调 数据: " + JsonConvert.SerializeObject(callbackDataDict));
        }
        catch (Exception exp) {
            log.Error(exp);
        }

        if (callbackDataDict.Count == 0) {
            throw new Exception("支付宝支付回调 end fail_001");
        }

        bool checkSuccess = alipayService.RSACheckV1(callbackDataDict);
        if (checkSuccess == false) {
            throw new Exception("支付宝支付回调 end fail_003");
        }

        string outTradeNo = callbackDataDict["out_trade_no"];
        if (string.IsNullOrWhiteSpace(outTradeNo)) {
            throw new Exception("支付宝支付回调 out_trade_no 为空！");
        }

        var orderPay = session.Query<LogisticsOrderPayInfo>()
            .Where(e => e.OutTradeNo == outTradeNo)
            .Take(1)
            .SingleOrDefault();
        if (orderPay == null) {
            throw new Exception($"支付宝支付回调 指定的 out_trade_no:{outTradeNo} 不存在！");
        }

        // 如果该笔支付已经标记为成功，就直接返回 success 就行
        if (orderPay.Status == PayStatus.Paid) {
            // throw new Exception($"支付宝支付回调 指定的支付 out_trade_no:{outTradeNo},之前已完成！");
            log.Error($"支付宝支付回调 指定的支付 out_trade_no:{outTradeNo},之前已完成！");
            return;
        }

        if (callbackDataDict.ContainsKey("total_amount") == false) {
            throw new Exception($"total_amount 参数未提供，out_trade_no: {outTradeNo}");
        }

        if (decimal.TryParse(callbackDataDict["total_amount"], out decimal payAmount) == false) {
            throw new Exception($"total_amount 参数格式错误，out_trade_no: {outTradeNo}");
        }

        orderPay.Amount = payAmount;
        orderPay.Status = PayStatus.Paid;
        session.Update(orderPay);
        session.Flush();

        // 执行积分抵扣
        if (orderPay.ScoreDeductBaseAmount > 0) {
            customerScoreDeductService.Deduct(orderPay.Order.Id, orderPay.ScoreDeductBaseAmount);
        }

        LogisticsOrderInfo order = orderPay.Order;

        PayStatus orderPayStatusBefore = order.PayStatus;

        // 合计标记支付状态
        logisticsOrderPaySumService.SumFeeAndUpdatePayStatus(order.Id);

        PayStatus orderPayStatusAfter = order.PayStatus;

        // 记录日志
        logisticsOrderActionService.NewActionWithoutUserCustomer(order.Id, LogisticsOrderActionType.Pay, orderPayStatusBefore, orderPayStatusAfter, $"支付宝 金额:{payAmount:#0.00}，支付单号:{orderPay.OutTradeNo}");

        // 会员积分
        customerScoreService.AddScore(orderPay.Id);

        log.Warn("支付宝支付回调 end success 2");
    }
}