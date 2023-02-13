using Microsoft.AspNetCore.Mvc;
using NLog;
using shunda.Core.Web;
using ISession = NHibernate.ISession;

namespace shunda.Client.Controllers;

[NoLoginRequired]
public class WechatAppCallbackController : Controller {
    private static Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;

    public WechatAppCallbackController(ISession session) {
        this.session = session;
    }

    private const string callbackUrl = "/client/wechat/app/callback";
    
}