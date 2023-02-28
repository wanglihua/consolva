using NHibernate;
using shunda.Core;
using shunda.Core.Config;
using shunda.Core.Service;
using shunda.Core.WechatPay;
using shunda.Domain.Fee;
using shunda.Domain.LogisticsOrder;
using ISession = NHibernate.ISession;

namespace shunda.Service.Pay;

[Service(ServiceType.Scoped)]
public class WechatPayOrderService {
    private readonly ISession session;
    private readonly PayService payService;

    public WechatPayOrderService(
        ISession session,
        PayService payService
    ) {
        this.session = session;
        this.payService = payService;
    }

    /// <summary>
    /// 创建订单支付
    /// 在数据库创建本地支付记录
    /// 在微信端创建 unified order
    /// </summary>
    /// <param name="orderId"></param>
    /// <param name="notifyUrl">回调URL，没有域名部分，比如 /client/wechat/app/charge/wechat/pay/notify</param>
    /// <returns></returns>
    /// <exception cref="ValidationException"></exception>
    public WechatPayData Create(int orderId, string notifyUrl) {
        LogisticsOrderInfo order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null) {
            throw new ValidationException($"指定的订单#{orderId}不存在！");
        }

        NHibernateUtil.Initialize(order.Customer);

        string outTradeNo = payService.GenerateOutTradeNo();

        LogisticsOrderPayInfo logisticsOrderPay = new LogisticsOrderPayInfo {
            Order = order,
            Customer = order.Customer,
            PayType = PayType.Wechat,
            OutTradeNo = outTradeNo,
            Amount = order.ArrearsAmountFormula,
            Status = PayStatus.NotPaid
        };

        session.Save(logisticsOrderPay);
        session.Flush();

        string timeStamp = WechatPayApi.GenerateTimeStamp();

        // 订单名称
        string subject = "订单" + order.Code + "_" + timeStamp;

        // 付款金额，单位是分
        int totalAmount = (int)(order.ArrearsAmountFormula * 100);

        // 商品描述
        string itemBody = subject + "物流" + order.FirstMode.Name;

        WechatPayData initData = new WechatPayData();
        initData.SetValue("appid", WechatAppConfig.AppId);
        initData.SetValue("body", subject); //商品描述
        initData.SetValue("attach", itemBody); //附加数据
        initData.SetValue("out_trade_no", outTradeNo);
        initData.SetValue("total_fee", totalAmount); //总金额
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

        WechatPayData unifiedData = new WechatPayData();
        unifiedData.SetValue("appId", result.GetValue("appid") ?? string.Empty);
        unifiedData.SetValue("timeStamp", WechatPayApi.GenerateTimeStamp());
        unifiedData.SetValue("nonceStr", WechatPayApi.GenerateNonceStr());
        unifiedData.SetValue("package", "prepay_id=" + result.GetValue("prepay_id"));
        unifiedData.SetValue("signType", WechatPayData.SIGN_TYPE_HMAC_SHA256);
        unifiedData.SetValue("paySign", unifiedData.MakeSign(WechatPayData.SIGN_TYPE_HMAC_SHA256));

        return unifiedData;
    }

    public string Notify(WechatPayData requestData) {
        return string.Empty;
    }
}

