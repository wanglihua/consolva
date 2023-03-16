using Alipay.AopSdk.AspnetCore;
using Newtonsoft.Json;
using NLog;
using shunda.Core.Service;
using shunda.Service.Customer;
using ISession = NHibernate.ISession;

namespace shunda.Service.Pay;

[Service(ServiceType.Scoped)]
public class AlipayChargeService {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;
    private readonly PayService payService;
    private readonly AlipayService alipayService;
    private readonly CustomerChargeService customerChargeService;

    public AlipayChargeService(
        ISession session,
        PayService payService,
        AlipayService alipayService,
        CustomerChargeService customerChargeService
    ) {
        this.session = session;
        this.payService = payService;
        this.alipayService = alipayService;
        this.customerChargeService = customerChargeService;
    }

    /// <summary>
    /// 创建支付宝支付返回 alipay request url
    /// </summary>
    /// <param name="param">参数对象</param>
    /// <returns>alipay request url 用于跳转支付宝支付</returns>
    public string Create(AlipayChargeCreateParam param) {
        return "";
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

        //交易状态
        //判断该笔订单是否在商户网站中已经做过处理
        //如果没有做过处理，根据订单号（out_trade_no）在商户网站的订单系统中查到该笔订单的详细，并执行商户的业务程序
        //请务必判断请求时的total_amount与通知时获取的total_fee为一致的
        //如果有做过处理，不执行商户的业务程序

        string outTradeNo = callbackDataDict["out_trade_no"];
    }
}
