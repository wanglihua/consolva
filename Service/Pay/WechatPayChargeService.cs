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
    /// <param name="notifyUrl">回调URL，没有域名部分，比如 /client/wechat/app/charge/wechat/pay/notify </param>
    /// <returns></returns>
    /// <exception cref="ValidationException"></exception>
    public WechatPayData Create(int customerId, decimal amount, string notifyUrl) {
        CustomerInfo customer = session.Get<CustomerInfo>(customerId);
        if (customer == null) {
            throw new ValidationException($"指定的客户#{customerId}不存在，请确认客户已登录！");
        }

        string outTradeNo = payService.GenerateOutTradeNo();
        CustomerChargeInfo customerCharge = new CustomerChargeInfo();
        customerCharge.ChargeType = CustomerChargeType.Wechat;
        customerCharge.Customer = customer;
        customerCharge.OutTradeNo = outTradeNo;
        customerCharge.Amount = amount;
        customerCharge.Status = CustomerChargeStatus.New;

        session.Save(customerCharge);
        session.Flush();

        // 订单名称
        string subject = "会员充值_" + customerId + "_" + customerCharge.Id;

        // 商品描述
        string itemBody = subject + "_" + amount.ToString("0.00");


        WechatPayData initData = new WechatPayData();
        initData.SetValue("body", subject); //商品描述
        initData.SetValue("attach", itemBody); //附加数据
        initData.SetValue("out_trade_no", outTradeNo);
        initData.SetValue("total_fee", amount); //总金额
        initData.SetValue("time_start", DateTime.Now.ToString("yyyyMMddHHmmss")); //交易起始时间
        initData.SetValue("time_expire", DateTime.Now.AddMinutes(30).ToString("yyyyMMddHHmmss")); //交易结束时间
        initData.SetValue("goods_tag", "customer charge"); //商品标记
        initData.SetValue("trade_type", "JSAPI");

        string openId = customer.WechatAppOpenId;
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

    /// <summary>
    /// 充值支付回调
    /// <returns>处理后的需要返回给微信的xml</returns>
    /// </summary>
    public string Notify(WechatPayData requestData) {
        try {
            string outTradeNo = requestData.GetValue("out_trade_no")?.ToString() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(outTradeNo)) {
                throw new WechatPayException("回调数据异常，out_trade_no 未设置！");
            }

            var customerCharge = session.Query<CustomerChargeInfo>()
                .Where(e => e.OutTradeNo == outTradeNo)
                .Take(1)
                .SingleOrDefault();
            if (customerCharge == null) {
                throw new WechatPayException($"out_trade_no 本地系统中不存在或有多个。out_trade_no：{outTradeNo}");
            }

            // 实际支付金额
            decimal payAmount = ((decimal)int.Parse(requestData.GetValue("total_fee")?.ToString() ?? string.Empty)) / 100m;

            //调统一下单接口，获得下单结果
            // string openid = requestData.GetValue("openid")?.ToString() ?? string.Empty;
            // WechatPayData unifiedOrderResult = UnifiedOrder(openid, customerCharge.Id); // 会员充值_customerId

            //若下单失败，则立即返回结果给微信支付后台
            // if (!unifiedOrderResult.IsSet("appid") || !unifiedOrderResult.IsSet("mch_id") || !unifiedOrderResult.IsSet("prepay_id")) {
            //     throw new WechatPayException("统一下单失败, appid 或 mch_id 或 prepay_id 未设置！");
            // }

            // 如果支付记录是【还没】支付的，就先做本地费数据处理；
            // 如果支付记录是【已经】支付的，就直接返回success
            if (customerCharge.Status == CustomerChargeStatus.New) {
                // 完成充值
                customerChargeService.CompleteCharge(customerCharge, payAmount, CustomerChargeType.Wechat);
            }

            //统一下单成功,则返回成功结果给微信支付后台
            WechatPayData data = new WechatPayData();
            data.SetValue("return_code", "SUCCESS");
            data.SetValue("return_msg", "OK");
            data.SetValue("appid", WechatAppConfig.AppId);
            data.SetValue("mch_id", WechatPayConfig.MchId);
            data.SetValue("nonce_str", WechatPayApi.GenerateNonceStr());
            data.SetValue("result_code", "SUCCESS");
            data.SetValue("err_code_des", "OK");
            data.SetValue("sign", data.MakeSign(WechatPayData.SIGN_TYPE_HMAC_SHA256));

            return data.ToXml();
        }
        catch (Exception exp) {
            log.Error(exp);

            WechatPayData data = new WechatPayData();
            data.SetValue("return_code", "FAIL");
            data.SetValue("return_msg", exp.Message);

            return data.ToXml();
        }
    }
}