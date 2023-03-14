using System.Text;
using Microsoft.AspNetCore.Mvc;
using NLog;
using shunda.Client.Areas.Pay.Models.WechatAppExpressReturnWechatPay;
using shunda.Client.Services;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Core.WechatPay;
using shunda.Domain.Express;
using shunda.Service.Pay;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Pay.Controllers;

/// <summary>
/// 退运支付
/// </summary>
[PayArea]
public class WechatAppExpressReturnWechatPayController : Controller {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;
    private readonly WechatPayExpressReturnService wechatPayExpressReturnService;

    public WechatAppExpressReturnWechatPayController(
        ISession session,
        LoginCustomerService loginCustomerService,
        WechatPayExpressReturnService wechatPayExpressReturnService
    ) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
        this.wechatPayExpressReturnService = wechatPayExpressReturnService;
    }

    [HttpPost]
    [Route("/client/wechat/app/express/return/wechat/pay/create")]
    [Transactional]
    public IActionResult Create([FromBody] WechatAppExpressReturnWechatPayCreateParam param) {
        ValidationUtil.Validate<WechatAppExpressReturnWechatPayCreateParamValidator, WechatAppExpressReturnWechatPayCreateParam>(param);

        int expressId = param.ExpressId;

        // 判断快递单是不是当前登录用户的
        var express = session.Get<ExpressInfo>(expressId);
        if (express == null) {
            throw new ValidationException($"指定的快递单号#{expressId}不存在！");
        }

        if (express.Customer == null) {
            throw new ValidationException($"指定的快递单号{express.TrackingNo}认领客户为空！");
        }

        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        if (express.Customer.Id != loginCustomerId) {
            throw new ValidationException($"指定的快递单号{express.TrackingNo}不属于当前登录客户#{loginCustomerId}");
        }

        WechatPayCreateResultData data = wechatPayExpressReturnService.Create(expressId, NOTIFY_URL);

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }

    private const string NOTIFY_URL = "/client/wechat/app/express/return/wechat/pay/notify";

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

            wechatPayExpressReturnService.Notify(requestData);

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