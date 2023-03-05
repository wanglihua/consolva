using NHibernate;
using NLog;
using shunda.Core;
using shunda.Core.Config;
using shunda.Core.Service;
using shunda.Core.WechatPay;
using shunda.Domain.Coupon;
using shunda.Domain.Fee;
using shunda.Domain.LogisticsOrder;
using shunda.Service.Coupon;
using shunda.Service.Customer;
using shunda.Service.Fee;
using shunda.Service.LogisticsOrder;
using ISession = NHibernate.ISession;

namespace shunda.Service.Pay;

[Service(ServiceType.Scoped)]
public class WechatPayOrderService {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;
    private readonly PayService payService;
    private readonly LogisticsOrderPaySumService logisticsOrderPaySumService;
    private readonly LogisticsOrderActionService logisticsOrderActionService;
    private readonly CustomerScoreService customerScoreService;
    private readonly CouponService couponService;

    public WechatPayOrderService(
        ISession session,
        PayService payService,
        LogisticsOrderPaySumService logisticsOrderPaySumService,
        LogisticsOrderActionService logisticsOrderActionService,
        CustomerScoreService customerScoreService,
        CouponService couponService
    ) {
        this.session = session;
        this.payService = payService;
        this.logisticsOrderPaySumService = logisticsOrderPaySumService;
        this.logisticsOrderActionService = logisticsOrderActionService;
        this.customerScoreService = customerScoreService;
        this.couponService = couponService;
    }

    /// <summary>
    /// 创建订单支付，调用之前确认订单是当前登录用户的
    /// 在数据库创建本地支付记录
    /// 在微信端创建 unified order
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="orderId"></param>
    /// <param name="couponId"></param>
    /// <param name="notifyUrl">回调URL，没有域名部分，比如 /client/wechat/app/charge/wechat/pay/notify</param>
    /// <returns></returns>
    /// <exception cref="ValidationException"></exception>
    public WechatPayCreateResultData Create(int customerId, int orderId, int couponId, string notifyUrl) {
        LogisticsOrderInfo order = session.Get<LogisticsOrderInfo>(orderId);
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

        NHibernateUtil.Initialize(order.Customer);

        decimal totalAmount = order.OrderAmount;

        decimal discountAmount = 0;
        if (coupon != null) {
            var discountResult = couponService.CalculateCouponDiscount(couponId, totalAmount);
            if (discountResult.Success == false) {
                discountAmount = 0;
            } else {
                discountAmount = discountResult.Discount;

                // 保存优惠券到订单，并生成响应的订单费用，负的费用
                couponService.SaveToOrder(customerId: customerId, orderId: orderId, couponId: couponId);
            }
        }

        decimal paidAmount = order.PaidAmount;
        decimal arrearsAmount = totalAmount - discountAmount - paidAmount;

        if (arrearsAmount < 0) {
            arrearsAmount = 0;
        }

        if (arrearsAmount <= 0) {
            // 如果欠费金额是0，就不用生成微信支付了，直接成功支付就可以了
            var orderPayZeroAmount = new LogisticsOrderPayInfo {
                Order = order,
                Customer = order.Customer,
                PayType = PayType.Wechat,
                OutTradeNo = string.Empty,
                Amount = 0, // 特殊情况的时候：订单用户原来支付过了一些，第二次支付的时候获得了优惠券折扣，这个折扣正好完全抵掉了欠款，这时就会生成一个支付金额为0的记录，同时把订单标记为已支付。
                Status = PayStatus.Paid
            };

            session.Save(orderPayZeroAmount);
            session.Flush();

            logisticsOrderPaySumService.SumFeeAndUpdatePayStatus(orderId); // 这个这里可以不调用，因为 couponService.SaveToOrder()里面已经调用过一次了

            return new WechatPayCreateResultData {
                NeedPay = false,
                TimeStamp = string.Empty,
                NonceStr = string.Empty,
                Package = string.Empty,
                SignType = string.Empty,
                PaySign = string.Empty,
            };
        }

        string outTradeNo = payService.GenerateOutTradeNo();
        var logisticsOrderPay = new LogisticsOrderPayInfo {
            Order = order,
            Customer = order.Customer,
            PayType = PayType.Wechat,
            OutTradeNo = outTradeNo,
            Amount = arrearsAmount,
            Status = PayStatus.NotPaid
        };

        session.Save(logisticsOrderPay);
        session.Flush();

        // 订单名称
        string subject = "订单" + order.Code + "_" + logisticsOrderPay.Id;

        // 商品描述
        string itemBody = "订单" + order.Code + "_" + logisticsOrderPay.Id;

        // 付款金额，单位是分
        int arrearsAmountCent = (int)(arrearsAmount * 100);

        string appId = WechatAppConfig.AppId;

        WechatPayData initData = new WechatPayData();
        initData.SetValue("appid", appId);
        initData.SetValue("body", subject); //商品描述
        initData.SetValue("attach", itemBody); //附加数据
        initData.SetValue("out_trade_no", outTradeNo);
        initData.SetValue("total_fee", arrearsAmountCent); //总金额
        initData.SetValue("time_start", DateTime.Now.ToString("yyyyMMddHHmmss")); //交易起始时间
        initData.SetValue("time_expire", DateTime.Now.AddMinutes(30).ToString("yyyyMMddHHmmss")); //交易结束时间
        initData.SetValue("goods_tag", "customer charge"); //商品标记
        initData.SetValue("trade_type", "JSAPI");

        string openId = order.Customer.WechatAppOpenId;
        initData.SetValue("openid", openId);

        initData.SetValue("notify_url", SiteConfig.SiteRoot + notifyUrl);

        WechatPayData result = WechatPayApi.UnifiedOrder(initData);

        if (!result.IsSet("appid") || !result.IsSet("prepay_id") || string.IsNullOrWhiteSpace(result.GetValue("prepay_id")?.ToString())) {
            throw new WechatPayException("wechat pay UnifiedOrder response error!");
        }

        string timeStamp = WechatPayApi.GenerateTimeStamp();
        string nonceStr = WechatPayApi.GenerateNonceStr();
        string package = "prepay_id=" + result.GetValue("prepay_id");
        string signType = WechatPayData.SIGN_TYPE_HMAC_SHA256;

        // 利用 WechatPayData 做签名
        WechatPayData unifiedData = new WechatPayData();
        unifiedData.SetValue("appId", appId);
        unifiedData.SetValue("timeStamp", timeStamp);
        unifiedData.SetValue("nonceStr", nonceStr);
        unifiedData.SetValue("package", package);
        unifiedData.SetValue("signType", signType);

        string paySign = unifiedData.MakeSign(WechatPayData.SIGN_TYPE_HMAC_SHA256);

        return new WechatPayCreateResultData {
            NeedPay = true,
            TimeStamp = timeStamp,
            NonceStr = nonceStr,
            Package = package,
            SignType = signType,
            PaySign = paySign,
        };
    }

    /// <summary>
    /// 订单支付回调
    /// </summary>
    public void Notify(WechatPayData requestData) {
        string outTradeNo = requestData.GetValue("out_trade_no")?.ToString() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(outTradeNo)) {
            throw new WechatPayException("回调数据异常，out_trade_no 未设置！");
        }

        var orderPay = session.Query<LogisticsOrderPayInfo>()
            .Where(e => e.OutTradeNo == outTradeNo)
            .Take(1)
            .SingleOrDefault();
        if (orderPay == null) {
            throw new WechatPayException($"out_trade_no 本地系统订单支付记录中不存在。out_trade_no：{outTradeNo}");
        }

        NHibernateUtil.Initialize(orderPay.Order);

        var order = orderPay.Order;
        int orderId = order.Id;

        // 实际支付金额
        string totalFeeValue = requestData.GetValue("total_fee")?.ToString() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(totalFeeValue)) {
            throw new WechatPayException("回调数据异常，total_fee 未设置！");
        }

        if (int.TryParse(totalFeeValue, out int payAmountInt) == false) {
            throw new WechatPayException("回调数据异常，total_fee 格式错误！");
        }

        // 分转成元
        decimal payAmount = ((decimal)payAmountInt) / 100m;

        if (orderPay.Status == PayStatus.NotPaid) {
            // 标记订单支付为已支付
            orderPay.Amount = payAmount;
            orderPay.Status = PayStatus.Paid;
            session.Update(orderPay);
            session.Flush();

            // 合计标记支付状态
            logisticsOrderPaySumService.SumFeeAndUpdatePayStatus(orderId);

            // 订单日志
            logisticsOrderActionService.NewActionWithoutUserCustomer(orderId, LogisticsOrderActionType.Pay, $"金额：{payAmount:#0.00)}, 微信单号：{orderPay.OutTradeNo}");

            // 会员积分
            customerScoreService.AddScore(orderPay.Id);
        }
    }
}