using Alipay.AopSdk.AspnetCore;
using Alipay.AopSdk.Core.Domain;
using Alipay.AopSdk.Core.Request;
using Newtonsoft.Json;
using NHibernate;
using NLog;
using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.Coupon;
using shunda.Domain.Fee;
using shunda.Domain.LogisticsOrder;
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

    public AlipayOrderService(
        ISession session,
        PayService payService,
        AlipayService alipayService,
        LogisticsOrderActionService logisticsOrderActionService,
        CustomerScoreService customerScoreService,
        LogisticsOrderPaySumService logisticsOrderPaySumService
    ) {
        this.session = session;
        this.payService = payService;
        this.alipayService = alipayService;
        this.logisticsOrderActionService = logisticsOrderActionService;
        this.customerScoreService = customerScoreService;
        this.logisticsOrderPaySumService = logisticsOrderPaySumService;
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

        LogisticsOrderPayInfo orderPay = new LogisticsOrderPayInfo {
            Order = order,
            Customer = order.Customer,
            PayType = PayType.AliPay,
            OutTradeNo = outTradeNo,
            Amount = order.ArrearsAmountFormula,
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
                TotalAmount = totalAmount,
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
                TotalAmount = totalAmount,
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

        LogisticsOrderInfo order = orderPay.Order;

        PayStatus orderPayStatusBefore = order.PayStatus;

        // 合计标记支付状态
        logisticsOrderPaySumService.SumFeeAndUpdatePayStatus(order.Id);

        PayStatus orderPayStatusAfter = order.PayStatus;

        // 记录日志
        logisticsOrderActionService.NewActionWithoutUserCustomer(order.Id, LogisticsOrderActionType.Pay, orderPayStatusBefore, orderPayStatusAfter, $"金额:{payAmount:#0.00}，支付单号:{orderPay.OutTradeNo}");

        // 会员积分
        customerScoreService.AddScore(orderPay.Id);

        log.Warn("支付宝支付回调 end success 2");
    }
}