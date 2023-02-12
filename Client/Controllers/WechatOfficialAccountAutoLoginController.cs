using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Senparc.NeuChar.NeuralSystems;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;
using shunda.Client.Services;
using shunda.Core.Config;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Customer;
using shunda.Service.Wechat;
using ISession = NHibernate.ISession;

namespace shunda.Client.Controllers;

[NoLoginRequired]
public class WechatOfficialAccountAutoLoginController : Controller {
    private readonly ISession session;
    private readonly WechatOfficialAccountSessionService wechatOfficialAccountSessionService;
    private readonly LoginCustomerService loginCustomerService;

    public WechatOfficialAccountAutoLoginController(ISession session,
        WechatOfficialAccountSessionService wechatOfficialAccountSessionService,
        LoginCustomerService loginCustomerService
    ) {
        this.session = session;
        this.wechatOfficialAccountSessionService = wechatOfficialAccountSessionService;
        this.loginCustomerService = loginCustomerService;
    }

    [HttpGet]
    [Route("/client/wechat/official/account/auto/login")]
    public IActionResult AutoLogin(string url) {
        string clientLoginUrl = "/client/login";

        // 如果当前是已经登录的
        if (loginCustomerService.IsLogined == true) {
            return Redirect(url);
        }

        if (WebUtil.IsWechatRequest(Request)) {
            return Redirect(clientLoginUrl);
        }

        string openId = wechatOfficialAccountSessionService.GetOpenId();
        // 有 open id
        if (string.IsNullOrWhiteSpace(openId) == false) {
            var customer = session.Query<CustomerInfo>()
                .Where(e => e.WechatOfficialAccountOpenId == openId)
                .Take(1)
                .SingleOrDefault();
            // 就是 open id 还没关联客户
            if (customer == null) {
                // 登录的时候会自动关联 open id
                return Redirect(clientLoginUrl);
            }
            
            // todo 这里参考 微信小程序自动登录

            // 使用 open id 自动登录
            LoginResult result = loginCustomerService.Login(customer, CustomerLoginStage.WechatOfficialAccountLogin, Request);
            if (result.ResultCode != LoginResultCode.Success) {
                return Redirect(clientLoginUrl);
            }

            return Redirect(url);
        }

        // 无 open id，就会走到这里
        string state = (new Random()).Next(100000, 999999).ToString();
        wechatOfficialAccountSessionService.SetState(state);

        string callbackUrl = $"{SiteConfig.SiteRoot}{WechatOfficialAccountOAuthController.CallbackUrl}?returnUrl={Request.GetEncodedUrl()}";
        string authorizeUrl = OAuthApi.GetAuthorizeUrl(WechatOfficialAccountConfig.AppId, callbackUrl, state, OAuthScope.snsapi_base);

        return Redirect(authorizeUrl);
    }
}