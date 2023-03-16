using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using NLog;
using shunda.Client.Areas.Pay.Models.WechatOfficialAccountChargeAlipay;
using shunda.Core;
using shunda.Core.Config;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Customer;
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
        // 检查参数正确性
        int customerId = payService.GetIntFromPayCodeInUrl(customerPayCode);
        int amountInt = payService.GetIntFromPayCodeInUrl(amountPayCode);

        var customer = session.Get<CustomerInfo>(customerId);
        if (customer == null) {
            throw new ValidationException($"指定的客户#{customerId}不存在！");
        }

        if (amountInt <= 0) {
            throw new ValidationException("充值金额不能为负或0！");
        }

        if (WebUtil.IsWechatRequest(Request)) {
            var model = new WechatOfficialAccountChargeAlipayCopyUrlModel {
                Url = Request.GetEncodedUrl(),
            };
            return View("WechatOfficialAccountChargeAlipayCopyUrl", model);
        }

        return RedirectToAction("AliPayRequest", new { customerPayCode = customerPayCode, amountPayCode = amountPayCode });
    }

    [HttpGet]
    [Route("/client/charge/alipay/request/{customerPayCode}/{amountPayCode}")]
    public IActionResult AlipayRequest(string customerPayCode, string amountPayCode) {
        if (WebUtil.IsWechatRequest(Request)) {
            return RedirectToAction("CopyUrl", new { customerPayCode = customerPayCode, amountPayCode = amountPayCode });
        }

        int customerId = payService.GetIntFromPayCodeInUrl(customerPayCode);
        int amountInt = payService.GetIntFromPayCodeInUrl(amountPayCode);

        bool isMobile = WebUtil.IsMobileRequest(Request);

        AlipayChargeCreateParam createParam = new AlipayChargeCreateParam {
            CustomerId = customerId,
            Amount = (decimal)amountInt / 100m,
            IsMobile = isMobile,
            ReturnUrl = SiteConfig.SiteUrlBase + Url.Action("Return", "WechatOfficialAccountChargeAlipay"),
            NotifyUrl = SiteConfig.SiteUrlBase + Url.Action("Notify", "WechatOfficialAccountChargeAlipay"),
        };

        string alipayRequestUrl = alipayChargeService.Create(createParam);
        return Redirect(alipayRequestUrl);
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