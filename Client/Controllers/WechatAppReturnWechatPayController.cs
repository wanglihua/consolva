using Microsoft.AspNetCore.Mvc;
using NLog;
using ISession = NHibernate.ISession;

namespace shunda.Client.Controllers;

/// <summary>
/// 退运支付
/// </summary>
public class WechatAppReturnWechatPayController : Controller {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;

    public WechatAppReturnWechatPayController(ISession session) {
        this.session = session;
    }
}