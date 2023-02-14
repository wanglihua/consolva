using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;
using shunda.Client.Services;
using shunda.Core.Config;
using shunda.Core.Web;
using shunda.Domain.Customer;
using shunda.Service.Customer;
using shunda.Service.Wechat;
using ISession = NHibernate.ISession;

namespace shunda.Client.Controllers;

[NoLoginRequired]
public class WechatOfficialAccountAutoLoginController : Controller {
    private readonly ISession session;
    private readonly WechatOfficialAccountSessionService wechatOfficialAccountSessionService;
    private readonly CustomerService customerService;
    private readonly LoginCustomerService loginCustomerService;
    private readonly WechatOfficialAccountFollowerService wechatOfficialAccountFollowerService;

    public WechatOfficialAccountAutoLoginController(ISession session,
        WechatOfficialAccountSessionService wechatOfficialAccountSessionService,
        CustomerService customerService,
        LoginCustomerService loginCustomerService,
        WechatOfficialAccountFollowerService wechatOfficialAccountFollowerService
    ) {
        this.session = session;
        this.wechatOfficialAccountSessionService = wechatOfficialAccountSessionService;
        this.customerService = customerService;
        this.loginCustomerService = loginCustomerService;
        this.wechatOfficialAccountFollowerService = wechatOfficialAccountFollowerService;
    }

    [HttpGet]
    [Route("/client/wechat/official/account/auto/login")]
    public IActionResult AutoLogin(string url) {
        // 如果当前是已经登录的
        if (loginCustomerService.IsLogined == true) {
            return Redirect(url);
        }

        string openId = wechatOfficialAccountSessionService.GetOpenId();
        // 有 open id
        if (string.IsNullOrWhiteSpace(openId) == false) {
            var customer = session.Query<CustomerInfo>()
                .Where(e => e.WechatOfficialAccountOpenId == openId)
                .Take(1)
                .SingleOrDefault();

            if (customer != null && string.IsNullOrWhiteSpace(customer.WechatUnionId)) {
                string unionId = wechatOfficialAccountFollowerService.GetUnionId(openId); // 这个结果可能为空
                if (string.IsNullOrWhiteSpace(unionId) == false) {
                    customer.WechatUnionId = unionId;

                    session.Update(customer);
                    session.Flush();
                }
            }

            // 就是 open id 还没关联客户
            if (customer == null) {
                string unionId = wechatOfficialAccountFollowerService.GetUnionId(openId); // 这个结果可能为空
                customer = customerService.CreateCustomerByOpenId(string.Empty, openId, unionId);
            }

            // 使用 open id 自动登录
            loginCustomerService.Login(customer, CustomerLoginStage.WechatOfficialAccountLogin, Request);

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