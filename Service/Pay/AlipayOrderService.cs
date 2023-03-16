using Alipay.AopSdk.AspnetCore;
using Alipay.AopSdk.Core.Domain;
using Alipay.AopSdk.Core.Request;
using NHibernate;
using NLog;
using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.Coupon;
using shunda.Domain.Fee;
using shunda.Domain.LogisticsOrder;

namespace shunda.Service.Pay;

[Service(ServiceType.Scoped)]
public class AlipayOrderService {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;
    private readonly PayService payService;
    private readonly AlipayService alipayService;

    public AlipayOrderService(
        ISession session,
        PayService payService,
        AlipayService alipayService
    ) {
        this.session = session;
        this.payService = payService;
        this.alipayService = alipayService;
    }

    /// <summary>
    /// 创建支付宝支付返回 alipay request url
    /// </summary>
    /// <param name="param">参数对象</param>
    /// <returns>alipay request url 用于跳转支付宝支付</returns>
    public string Create(AlipayOrderCreateParam param) {
        log.Warn("支付宝支付发起 begin");

        int orderId = param.OrderId;
        int couponId = param.CouponId;

        var order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null) {
            throw new ValidationException($"指定的订单#{orderId}不存在！");
        }

        var coupon = session.Get<CouponInfo>(couponId);
        if (coupon == null) {
            throw new ValidationException($"指定的优惠券{couponId}不存在！");
        }

        if (order.ArrearsAmountFormula <= 0) {
            throw new ValidationException("当前订单无欠费金额，无需支付！");
        }

        // 外部订单号，商户网站订单系统中唯一的订单号
        string outTradeNo = payService.GenerateOutTradeNo();

        // 付款金额
        string totalAmount = order.ArrearsAmountFormula.ToString("#0.00");

        LogisticsOrderPayInfo orderPay = new LogisticsOrderPayInfo();
        orderPay.Order = order;
        orderPay.Customer = order.Customer;
        orderPay.PayType = PayType.AliPay;
        orderPay.OutTradeNo = outTradeNo;
        orderPay.Amount = order.ArrearsAmountFormula;
        orderPay.Status = PayStatus.NotPaid;
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
                TotalAmount = totalAmount,
                OutTradeNo = outTradeNo,
                ProductCode = "FAST_INSTANT_TRADE_PAY"
            };

            AlipayTradePagePayRequest payRequest = new AlipayTradePagePayRequest();
            // 设置同步回调地址
            payRequest.SetReturnUrl(param.CallbackUrl);
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
                TotalAmount = totalAmount,
                OutTradeNo = outTradeNo,
                ProductCode = "QUICK_WAP_WAY"
            };

            AlipayTradeWapPayRequest payRequest = new AlipayTradeWapPayRequest();
            // 设置同步回调地址
            payRequest.SetReturnUrl(param.CallbackUrl);
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
}