using NLog;
using shunda.Core;
using shunda.Core.Config;
using shunda.Core.Service;
using shunda.Core.WechatPay;
using shunda.Domain.Customer;
using shunda.Service.Customer;
using ISession = NHibernate.ISession;

namespace shunda.Service.Pay;

[Service(ServiceType.Scoped)]
public class WechatPayChargeService {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;
    private readonly PayService payService;
    private readonly CustomerChargeService customerChargeService;

    public WechatPayChargeService(
        ISession session,
        PayService payService,
        CustomerChargeService customerChargeService
    ) {
        this.session = session;
        this.payService = payService;
        this.customerChargeService = customerChargeService;
    }

    /// <summary>
    /// 创建充值支付
    /// 在数据库创建本地支付记录
    /// 在微信端创建 unified order
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="amount"></param>
    /// <param name="notifyUrl">回调URL，没有域名部分，比如 /client/wechat/app/charge/wechat/pay/notify</param>
    /// <returns></returns>
    /// <exception cref="ValidationException"></exception>
    public WechatPayCreateResultData Create(int customerId, decimal amount, string notifyUrl) {
        CustomerInfo customer = session.Get<CustomerInfo>(customerId);
        if (customer == null) {
            throw new ValidationException($"指定的客户#{customerId}不存在，请确认客户已登录！");
        }

        string outTradeNo = payService.GenerateOutTradeNo();

        var customerCharge = new CustomerChargeInfo {
            ChargeType = CustomerChargeType.Wechat,
            Customer = customer,
            OutTradeNo = outTradeNo,
            Amount = amount,
            Status = CustomerChargeStatus.New
        };

        session.Save(customerCharge);
        session.Flush();

        // 订单名称
        string subject = "会员充值_" + customerId + "_" + customerCharge.Id;

        // 商品描述
        string itemBody = "会员充值_" + customerId + "_" + customerCharge.Id;

        string appId = WechatAppConfig.AppId;

        WechatPayData initData = new WechatPayData();
        initData.SetValue("appid", appId);
        initData.SetValue("body", subject); //商品描述
        initData.SetValue("attach", itemBody); //附加数据
        initData.SetValue("out_trade_no", outTradeNo);
        initData.SetValue("total_fee", (int)(amount * 100)); //总金额
        initData.SetValue("time_start", DateTime.Now.ToString("yyyyMMddHHmmss")); //交易起始时间
        initData.SetValue("time_expire", DateTime.Now.AddMinutes(30).ToString("yyyyMMddHHmmss")); //交易结束时间
        initData.SetValue("goods_tag", "customer charge"); //商品标记
        initData.SetValue("trade_type", "JSAPI");

        string openId = customer.WechatAppOpenId;
        initData.SetValue("openid", openId);

        initData.SetValue("notify_url", SiteConfig.SiteUrlBase + notifyUrl);

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
            TimeStamp = timeStamp,
            NonceStr = nonceStr,
            Package = package,
            SignType = signType,
            PaySign = paySign,
        };
    }

    /// <summary>
    /// 充值支付回调
    /// </summary>
    public void Notify(WechatPayData requestData) {
        string outTradeNo = requestData.GetValue("out_trade_no")?.ToString() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(outTradeNo)) {
            throw new WechatPayException("回调数据异常，out_trade_no 未设置！");
        }

        var customerCharge = session.Query<CustomerChargeInfo>()
            .Where(e => e.OutTradeNo == outTradeNo)
            .Take(1)
            .SingleOrDefault();
        if (customerCharge == null) {
            throw new WechatPayException($"out_trade_no 本地系统客户充值支付记录中不存在。out_trade_no：{outTradeNo}");
        }

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

        if (customerCharge.Status == CustomerChargeStatus.New) {
            // 完成充值
            customerChargeService.CompleteCharge(customerCharge, payAmount, CustomerChargeType.Wechat);
        }
    }
}