using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;
using shunda.Client.Areas.WechatOfficialAccount.Controllers;
using shunda.Client.Services;
using shunda.Core.Config;
using shunda.Core.Service;
using shunda.Domain.Customer;
using shunda.Domain.Wechat;
using shunda.Service.Customer;
using shunda.Service.Wechat;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.WechatOfficialAccount.Filters;

/// <summary>
/// 在需要微信公众号自动登录的Action上放上 [ServiceFilter(typeof(WechatOfficialAccountAutoLoginFilter))]
/// </summary>
[Service(ServiceType.Scoped)]
public class WechatOfficialAccountAutoLoginFilter : ActionFilterAttribute {
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;
    private readonly WechatOfficialAccountSessionService wechatOfficialAccountSessionService;
    private readonly CustomerService customerService;

    public WechatOfficialAccountAutoLoginFilter(ISession session,
        LoginCustomerService loginCustomerService,
        WechatOfficialAccountSessionService wechatOfficialAccountSessionService,
        CustomerService customerService
    ) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
        this.wechatOfficialAccountSessionService = wechatOfficialAccountSessionService;
        this.customerService = customerService;
    }

    public override void OnActionExecuting(ActionExecutingContext context) {
        string url = context.HttpContext.Request.GetEncodedUrl();

        // 如果当前是已经登录的
        if (loginCustomerService.IsLogined == true) {
            // 不用重定向，context.HttpContext.Response.Redirect(url);
            return;
        }

        string openId = wechatOfficialAccountSessionService.GetOpenId();
        // 有 open id
        if (string.IsNullOrWhiteSpace(openId) == false) {
            var customer = session.Query<CustomerInfo>()
                .Where(e => e.WechatOfficialAccountOpenId == openId)
                .Take(1)
                .SingleOrDefault();

            var follower = session.Query<WechatOfficialAccountFollowerInfo>()
                .Where(e => e.OpenId == openId)
                .Take(1)
                .SingleOrDefault();

            if (follower == null) {
                throw new Exception("您还没有关注微信公众号，请先关注微信公众号再访问！");
            }

            if (customer != null && string.IsNullOrWhiteSpace(customer.WechatUnionId)) {
                customer.WechatUnionId = follower.UnionId;

                session.Update(customer);
                session.Flush();
            }

            // 就是 open id 还没关联客户
            if (customer == null) {
                customer = customerService.CreateCustomerByWechatOfficialAccount(follower);
            }

            // 使用 open id 自动登录
            loginCustomerService.Login(customer, CustomerLoginStage.WechatOfficialAccountLogin, context.HttpContext.Request);

            // 不用重定向，context.HttpContext.Response.Redirect(url);
            return;
        }

        // 无 open id，就会走到这里
        string state = (new Random()).Next(100000, 999999).ToString();
        wechatOfficialAccountSessionService.SetState(state);

        string callbackUrl = $"{SiteConfig.SiteUrlBase}{WechatOfficialAccountOAuthController.CallbackUrl}?returnUrl={context.HttpContext.Request.GetEncodedUrl()}";
        string authorizeUrl = OAuthApi.GetAuthorizeUrl(WechatOfficialAccountConfig.AppId, callbackUrl, state, OAuthScope.snsapi_base);

        context.HttpContext.Response.Redirect(authorizeUrl);
        return;
    }

    public override void OnActionExecuted(ActionExecutedContext context) {
    }
}