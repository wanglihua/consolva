using Microsoft.AspNetCore.Mvc;
using NLog;
using shunda.Client.Models.WechatAppChargeWechatPay;
using shunda.Client.Services;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using ISession = NHibernate.ISession;

namespace shunda.Client.Controllers;

/// <summary>
/// 充值支付
/// </summary>
public class WechatAppChargeWechatPayController : Controller {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;

    public WechatAppChargeWechatPayController(
        ISession session,
        LoginCustomerService loginCustomerService
    ) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
    }

    [HttpPost]
    [Route("/client/wechat/app/charge/wechat/pay/create")]
    [Transactional]
    public IActionResult Create([FromBody] WechatAppChargeWechatPayCreateParam param) {
        ValidationUtil.Validate<WechatAppChargeWechatPayCreateParamValidator, WechatAppChargeWechatPayCreateParam>(param);

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }

    [NoLoginRequired]
    [HttpPost]
    [Route("/client/wechat/app/charge/wechat/pay/notify")]
    [Transactional]
    public IActionResult Notify() {
        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }
}