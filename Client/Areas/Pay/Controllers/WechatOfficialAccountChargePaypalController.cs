using System.Text;
using Microsoft.AspNetCore.Mvc;
using NLog;
using PayPal;
using PayPal.Api;
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
    
    [NoLoginRequired]
    [HttpGet]
    [Route(CHARGE_PAYPAL_URL)]
    public IActionResult PaypalRequest(string customerPayCode, string amountPayCode) {
        int customerId = payService.GetIntFromPayCodeInUrl(customerPayCode);
        int amountInt = payService.GetIntFromPayCodeInUrl(amountPayCode);

        string outTradeNo = payService.GenerateOutTradeNo();

        var createParam = new PaypalChargeCreateParam {
            CustomerId = customerId,
            Amount = (decimal)amountInt / 100m,
            PaymentMethod = PaypalPaymentMethod.Paypal,

            ReturnUrl = SiteConfig.SiteUrlBase + Url.Action("Process", "WechatOfficialAccountChargePaypal") + $"?{shundaKey}={outTradeNo}",
            CancelUrl = SiteConfig.SiteUrlBase + Url.Action("Cancel", "WechatOfficialAccountChargePaypal"),
        };

        string approvalUrl = paypalChargeService.Create(createParam);
        return Redirect(approvalUrl);
    }

    [NoLoginRequired]
    [HttpGet]
    [Route("/client/order/paypal/process")]
    public IActionResult Process() {
        string payerId = Request.Query["PayerID"];
        string outTradeNo = Request.Query[shundaKey];

        bool hasPaypalError = false;
        string paymentId = string.Empty;

        try {
            var processParam = new PaypalChargeProcessParam {
                PayerId = payerId,
                OutTradeNo = outTradeNo,
            };
            paymentId = paypalChargeService.Process(processParam);
        }
        catch (PaymentsException exp) {
            hasPaypalError = true;

            var detailError = exp.Details;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine("   Error:    " + detailError.name);
            sb.AppendLine("   Message:  " + detailError.message);
            sb.AppendLine("   URI:      " + detailError.information_link);
            sb.AppendLine("   Debug ID: " + detailError.debug_id);

            if (detailError.details != null) {
                foreach (ErrorDetails detail in detailError.details) {
                    sb.AppendLine("   Details:  " + detail.field + " -> " + detail.issue);
                }
            }

            log.Error("PaymentsException error details: ");
            log.Error(sb.ToString());

            log.Error(exp);
        }
        catch (ConnectionException exp) {
            hasPaypalError = true;

            log.Error("ConnectionException response:");
            log.Error(exp.Response);

            log.Error(exp);
        }
        catch (PayPalException exp) {
            hasPaypalError = true;

            log.Error(exp);
        }
        catch (Exception exp) {
            log.Error(exp);
            throw;
        }

        var model = new WechatOfficialAccountChargePaypalProcessModel {
            HasPaypalError = hasPaypalError,
            PaymentId = paymentId,
        };

        return View("WechatOfficialAccountChargePaypalProcess", model);
    }

    [NoLoginRequired]
    [Route("/client/order/paypal/cancel")]
    public IActionResult Cancel() {
        var model = new WechatOfficialAccountChargePaypalCancelModel();
        return View("WechatOfficialAccountChargePaypalCancel", model);
    }    
}