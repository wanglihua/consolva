using Microsoft.AspNetCore.Mvc;
using NLog;
using ISession = NHibernate.ISession;

namespace shunda.Client.Controllers;

/// <summary>
/// 充值支付
/// </summary>
public class WechatAppChargeWechatPayController : Controller {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;

    public WechatAppChargeWechatPayController(ISession session) {
        this.session = session;
    }
}