using Microsoft.AspNetCore.Mvc.Filters;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.WechatOfficialAccount.Filters;

/// <summary>
/// 在需要微信公众号自动登录的Action上放上 [ServiceFilter(typeof(WechatOfficialAccountAutoLoginFilter))]
/// </summary>
public class WechatOfficialAccountAutoLoginFilter : ActionFilterAttribute {
    private readonly ISession session;

    public WechatOfficialAccountAutoLoginFilter(ISession session) {
        this.session = session;
    }

    public override void OnActionExecuting(ActionExecutingContext context) {
    }

    public override void OnActionExecuted(ActionExecutedContext context) {
    }
}