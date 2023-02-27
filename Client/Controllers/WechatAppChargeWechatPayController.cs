using System.Text;
using Microsoft.AspNetCore.Mvc;
using NLog;
using shunda.Client.Models.WechatAppChargeWechatPay;
using shunda.Client.Services;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Core.WechatPay;
using shunda.Service.Pay;
using ISession = NHibernate.ISession;

namespace shunda.Client.Controllers;

/// <summary>
/// 充值支付
/// </summary>
public class WechatAppChargeWechatPayController : Controller {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;
    private readonly WechatPayChargeService wechatPayChargeService;

    public WechatAppChargeWechatPayController(
        ISession session,
        LoginCustomerService loginCustomerService,
        WechatPayChargeService wechatPayChargeService
    ) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
        this.wechatPayChargeService = wechatPayChargeService;
    }

    [HttpPost]
    [Route("/client/wechat/app/charge/wechat/pay/create")]
    [Transactional]
    public IActionResult Create([FromBody] WechatAppChargeWechatPayCreateParam param) {
        ValidationUtil.Validate<WechatAppChargeWechatPayCreateParamValidator, WechatAppChargeWechatPayCreateParam>(param);

        int loginCustomerId = loginCustomerService.GetLoginCustomerId();
        WechatPayData data = wechatPayChargeService.Create(loginCustomerId, param.Amount, NOTIFY_URL);

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new WechatAppChargeWechatPayCreateModel {
                TimeStamp = data.GetValue("TimeStamp")?.ToString() ?? string.Empty,
                NonceStr = data.GetValue("noncestr")?.ToString() ?? string.Empty,
                Package = data.GetValue("package")?.ToString() ?? string.Empty,
                SignType = data.GetValue("signtype")?.ToString() ?? string.Empty,
                PaySign = data.GetValue("paysign")?.ToString() ?? string.Empty,
            }
        });
    }

    private const string NOTIFY_URL = "/client/wechat/app/charge/wechat/pay/notify";

    [NoLoginRequired]
    [HttpPost]
    [Route(NOTIFY_URL)]
    [Transactional]
    public IActionResult Notify() {
        StreamReader streamReader = new StreamReader(Request.Body);
        string requestBody = streamReader.ReadToEndAsync().GetAwaiter().GetResult();
        WechatPayData requestData = new WechatPayData();
        requestData.FromXml(requestBody);

        string xml = wechatPayChargeService.Notify(requestData);
        return Content(xml, "text/xml", Encoding.UTF8);
    }
}