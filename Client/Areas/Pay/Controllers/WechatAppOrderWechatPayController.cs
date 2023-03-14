using System.Text;
using Microsoft.AspNetCore.Mvc;
using NLog;
using shunda.Client.Areas.Pay.Models.WechatAppOrderWechatPay;
using shunda.Client.Services;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Core.WechatPay;
using shunda.Domain.LogisticsOrder;
using shunda.Service.Pay;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Pay.Controllers;

/// <summary>
/// 订单支付
/// </summary>
[PayArea]
public class WechatAppOrderWechatPayController : Controller {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;
    private readonly WechatPayOrderService wechatPayOrderService;

    public WechatAppOrderWechatPayController(
        ISession session,
        LoginCustomerService loginCustomerService,
        WechatPayOrderService wechatPayOrderService
    ) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
        this.wechatPayOrderService = wechatPayOrderService;
    }

    [HttpPost]
    [Route("/client/wechat/app/order/wechat/pay/create")]
    [Transactional]
    public IActionResult Create([FromBody] WechatAppOrderWechatPayCreateParam param) {
        ValidationUtil.Validate<WechatAppOrderWechatPayCreateParamValidator, WechatAppOrderWechatPayCreateParam>(param);

        int orderId = param.OrderId;
        int couponId = param.CouponId;

        // 判断订单是不是当前登录用户的
        var order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null) {
            throw new ValidationException($"指定的订单#{orderId}不存在！");
        }

        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        if (order.Customer.Id != loginCustomerId) {
            throw new ValidationException($"指定的订单{order.Code}不属于当前登录客户#{loginCustomerId}！");
        }

        WechatPayCreateResultData data = wechatPayOrderService.Create(customerId: loginCustomerId, orderId: orderId, couponId: couponId, NOTIFY_URL);

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = data,
        });
    }

    private const string NOTIFY_URL = "/client/wechat/app/order/wechat/pay/notify";

    [NoLoginRequired]
    [HttpPost]
    [Route(NOTIFY_URL)]
    [Transactional]
    public IActionResult Notify() {
        try {
            StreamReader streamReader = new StreamReader(Request.Body);
            string requestBody = streamReader.ReadToEndAsync().GetAwaiter().GetResult();

            WechatPayData requestData = new WechatPayData();
            requestData.FromXml(requestBody);

            wechatPayOrderService.Notify(requestData);

            WechatPayData data = new WechatPayData();
            data.SetValue("return_code", "SUCCESS");
            data.SetValue("return_msg", "OK");

            return Content(data.ToXml(), "text/xml", Encoding.UTF8);
        }
        catch (Exception exp) {
            log.Error(exp);

            WechatPayData data = new WechatPayData();
            data.SetValue("return_code", "FAIL");
            data.SetValue("return_msg", exp.Message);

            return Content(data.ToXml(), "text/xml", Encoding.UTF8);
        }
    }
}