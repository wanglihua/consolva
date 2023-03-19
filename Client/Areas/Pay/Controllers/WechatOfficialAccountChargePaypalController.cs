using Microsoft.AspNetCore.Mvc;
using NLog;
using shunda.Client.Areas.Pay.Models.WechatOfficialAccountChargePaypal;
using shunda.Client.Services;
using shunda.Core.Config;
using shunda.Core.Web;
using shunda.Service.Pay;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Pay.Controllers;

[PayArea]
public class WechatOfficialAccountChargePaypalController : Controller {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private static readonly string shundaKey = "shunda_key";

    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;
    private readonly PayService payService;
    private readonly PaypalChargeService paypalChargeService;

    public WechatOfficialAccountChargePaypalController(
        ISession session,
        LoginCustomerService loginCustomerService,
        PayService payService,
        PaypalChargeService paypalChargeService
    ) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
        this.payService = payService;
        this.paypalChargeService = paypalChargeService;
    }

    private const string CHARGE_PAYPAL_URL = "/client/charge/paypal/{customerPayCode}/{amountPayCode}";

    [HttpPost]
    [Route("/client/charge/alipay/create/url")]
    public IActionResult CreateUrl([FromBody] WechatOfficialAccountChargePaypalCreateUrlParam param) {
        int customerId = loginCustomerService.GetLoginCustomerId();
        decimal amount = param.Amount;
        int amountInt = (int)(amount * 100); // 转成 int 分

        string customerPayCode = payService.GenereatePayCodeInUrl(customerId);
        string amountPayCode = payService.GenereatePayCodeInUrl(amountInt);

        string url = CHARGE_PAYPAL_URL.Replace("{customerPayCode}", customerPayCode);
        url = url.Replace("{amountPayCode}", amountPayCode);
        url = SiteConfig.SiteUrlBase + url;

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new WechatOfficialAccountChargePaypalCreateUrlModel {
                Url = url,
            },
        });
    }    
}