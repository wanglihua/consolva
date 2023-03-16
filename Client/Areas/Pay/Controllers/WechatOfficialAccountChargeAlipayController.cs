using Microsoft.AspNetCore.Mvc;
using NLog;
using shunda.Client.Areas.Pay.Models.WechatOfficialAccountChargeAlipay;
using shunda.Core.Hibernate;
using shunda.Core.Web;
using shunda.Service.Pay;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Pay.Controllers;

[NoLoginRequired]
[PayArea]
public class WechatOfficialAccountChargeAlipayController : Controller {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;
    private readonly PayService payService;
    private readonly AlipayChargeService alipayChargeService;

    public WechatOfficialAccountChargeAlipayController(
        ISession session,
        PayService payService,
        AlipayChargeService alipayChargeService
    ) {
        this.session = session;
        this.payService = payService;
        this.alipayChargeService = alipayChargeService;
    }

    public const string CHARGE_ALIPAY_URL = "/client/charge/alipay/{customerPayCode}/{amountPayCode}";

    /// <summary>
    /// 用于复制粘贴到浏览器的URL
    /// </summary>
    /// <param name="customerPayCode">客户ID加密后的代码</param>
    /// <param name="amountPayCode">充值金额乘以100后的整数加密后的代码</param>
    /// <returns></returns>
    [HttpGet]
    [Route(CHARGE_ALIPAY_URL)]
    public IActionResult CopyUrl(string customerPayCode, string amountPayCode) {
        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }

    [HttpGet]
    [Route("/client/charge/alipay/request/{customerPayCode}/{amountPayCode}")]
    public IActionResult AlipayRequest() {
        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }

    [HttpPost]
    [Route("/client/charge/alipay/notify")]
    public IActionResult Notify() {
        Dictionary<string, string> callbackDataDict = new Dictionary<string, string>();

        ICollection<string> requestItem = Request.Form.Keys;
        foreach (var item in requestItem) {
            callbackDataDict.Add(item, Request.Form[item]);
        }

        try {
            Transactional.Wrap(session, () => { alipayChargeService.Callback(callbackDataDict); });
            return Content("success");
        }
        catch (Exception exp) {
            log.Error(exp);
            log.Error("支付宝支付异步回调 end fail_004");

            return Content("fail");
        }
    }
    
    [HttpGet]
    [Route("/client/charge/alipay/return")]
    public IActionResult Return() {
        Dictionary<string, string> callbackDataDict = new Dictionary<string, string>();

        ICollection<string> requestItem = Request.Form.Keys;
        foreach (var item in requestItem) {
            callbackDataDict.Add(item, Request.Form[item]);
        }

        try {
            Transactional.Wrap(session, () => { alipayChargeService.Callback(callbackDataDict); });
            var model = new WechatOfficialAccountChargeAlipayReturnModel {
                Success = true,
                Message = "支付成功！",
            };
            return View("WechatOfficialAccountChargeAlipayReturn", model);
        }
        catch (Exception exp) {
            log.Error(exp);
            log.Error("支付宝支付同步回调 end fail_004");

            var model = new WechatOfficialAccountChargeAlipayReturnModel {
                Success = false,
                Message = "支付失败！",
            };
            return View("WechatOfficialAccountChargeAlipayReturn", model);
        }
    }    
}