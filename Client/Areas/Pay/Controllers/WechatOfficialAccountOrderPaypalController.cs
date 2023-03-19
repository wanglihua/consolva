using Microsoft.AspNetCore.Mvc;
using NLog;
using shunda.Client.Areas.Pay.Models.WechatOfficialAccountOrderPaypal;
using shunda.Client.Services;
using shunda.Core;
using shunda.Core.Config;
using shunda.Core.Web;
using shunda.Domain.LogisticsOrder;
using shunda.Service.Pay;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Pay.Controllers;

/// <summary>
/// 公众号 PayPal支付 处理支付接口
/// </summary>
[NoLoginRequired]
[PayArea]
public class WechatOfficialAccountOrderPaypalController : Controller {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private static readonly string shundaKey = "shunda_key";

    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;
    private readonly PayService payService;
    private readonly PaypalOrderService paypalOrderService;

    public WechatOfficialAccountOrderPaypalController(
        ISession session,
        LoginCustomerService loginCustomerService,
        PayService payService,
        PaypalOrderService paypalOrderService
    ) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
        this.payService = payService;
        this.paypalOrderService = paypalOrderService;
    }

    private const string ORDER_PAYPAL_URL = "/client/order/paypal/{orderPayCode}/{couponPayCode}";

    [HttpPost]
    [Route("/client/order/paypal/create/url")]
    public IActionResult CreateUrl([FromBody] WechatOfficialAccountOrderPaypalCreateUrlParam param) {
        int orderId = param.OrderId;
        int couponId = param.CouponId;

        int customerId = loginCustomerService.GetLoginCustomerId();

        var order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null) {
            throw new ValidationException($"指定的订单#{orderId}不存在！");
        }

        if (order.Customer.Id != customerId) {
            throw new ValidationException($"指定的订单{order.Code}不是当前登录用户的！");
        }

        string orderPayCode = payService.GenereatePayCodeInUrl(orderId);
        string couponPayCode = payService.GenereatePayCodeInUrl(couponId);

        string url = ORDER_PAYPAL_URL.Replace("{orderPayCode}", orderPayCode);
        url = url.Replace("{couponPayCode}", couponPayCode);
        url = SiteConfig.SiteUrlBase + url;

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new WechatOfficialAccountOrderPaypalCreateUrlModel {
                Url = url,
            }
        });
    }

    [HttpGet]
    [Route(ORDER_PAYPAL_URL)]
    public IActionResult PaypalRequest(string orderPayCode, string couponPayCode) {
        int orderId = payService.GetIntFromPayCodeInUrl(orderPayCode);
        int couponId = payService.GetIntFromPayCodeInUrl(couponPayCode);

        var createParam = new PaypalOrderCreateParam {
            OrderId = orderId,
            CouponId = couponId,
            PaymentMethod = PaypalPaymentMethod.Paypal,

            ReturnUrl = SiteConfig.SiteUrlBase + Url.Action("Process", "WechatOfficialAccountOrderPaypal"),
            CancelUrl = SiteConfig.SiteUrlBase + Url.Action("Cancel", "WechatOfficialAccountOrderPaypal"),
        };

        string approvalUrl = paypalOrderService.Create(createParam);
        return Redirect(approvalUrl);
    }

    [HttpGet]
    [Route("/client/order/paypal/process")]
    public IActionResult Process() {
        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }

    [Route("/client/order/paypal/cancel")]
    public IActionResult Cancel() {
        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }
}