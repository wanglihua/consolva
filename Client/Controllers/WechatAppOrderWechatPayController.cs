using System.Text;
using Microsoft.AspNetCore.Mvc;
using NLog;
using shunda.Client.Models.WechatAppOrderWechatPay;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Core.WechatPay;
using ISession = NHibernate.ISession;

namespace shunda.Client.Controllers;

/// <summary>
/// 订单支付
/// </summary>
public class WechatAppOrderWechatPayController : Controller {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;

    public WechatAppOrderWechatPayController(ISession session) {
        this.session = session;
    }

    [HttpPost]
    [Route("/client/wechat/app/order/wechat/pay/create")]
    [Transactional]
    public IActionResult Create([FromBody] WechatAppOrderWechatPayCreateParam param) {
        ValidationUtil.Validate<WechatAppOrderWechatPayCreateParamValidator, WechatAppOrderWechatPayCreateParam>(param);


        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }

    [NoLoginRequired]
    [HttpPost]
    [Route("/client/wechat/app/order/wechat/pay/notify")]
    [Transactional]
    public IActionResult Notify() {
        try {
            StreamReader streamReader = new StreamReader(Request.Body);
            string requestBody = streamReader.ReadToEndAsync().GetAwaiter().GetResult();

            WechatPayData requestData = new WechatPayData();
            requestData.FromXml(requestBody);
            
            // notify
            
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