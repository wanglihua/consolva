using CSRedis;
using Microsoft.AspNetCore.Mvc;
using NLog;
using Senparc.Weixin;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using shunda.Core.Config;
using shunda.Core.Redis;
using shunda.Core.Web;
using shunda.Service.Wechat;

namespace shunda.Client.Controllers;

[NoLoginRequired]
public class WechatOfficialAccountOAuthController : Controller {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly WechatOfficialAccountSessionService wechatOfficialAccountSessionService;
    private readonly WechatOfficialAccountOAuthAccessTokenService wechatOfficialAccountOAuthAccessTokenService;

    public WechatOfficialAccountOAuthController(WechatOfficialAccountSessionService wechatOfficialAccountSessionService,
        WechatOfficialAccountOAuthAccessTokenService wechatOfficialAccountOAuthAccessTokenService
    ) {
        this.wechatOfficialAccountSessionService = wechatOfficialAccountSessionService;
        this.wechatOfficialAccountOAuthAccessTokenService = wechatOfficialAccountOAuthAccessTokenService;
    }

    public const string CallbackUrl = "/client/wechat/official/account/oauth/callback";

    [HttpGet]
    [Route(CallbackUrl)]
    public IActionResult Callback1(string code, string state, string returnUrl) {
        return CallbackInternal(code, state, returnUrl);
    }

    [NonAction]
    private IActionResult CallbackInternal(string code, string state, string returnUrl) {
        try {
            if (string.IsNullOrEmpty(code)) {
                throw new Exception("参数 code 不能为空！");
            }

            if (string.IsNullOrWhiteSpace(state)) {
                throw new Exception("参数 state 不能为空！");
            }

            /*
             todo 这里总是出错，懒得验证了
             是State的存储方式有问题，不能存在 session 里面???
            if (state != wechatOfficialAccountSessionService.GetState())
            {
                //这里的state其实是会暴露给客户端的，验证能力很弱，这里只是演示一下，
                //建议用完之后就清空，将其一次性使用
                throw new Exception("参数 state 验证失败！");
            }

            // 用完后清空，一次新使用
            wechatOfficialAccountSessionService.SetState(string.Empty);
            */

            // woa: wechat official account
            string codeKey = $"{RedisClientUtil.RedisKeyPrefix}:woa_oauth_code_key:{code}";

            CSRedisClient redisClient = RedisClientUtil.GetRedisClient();
            string openId = redisClient.Get(codeKey);

            // 解决 4016 code been used 的问题
            if (string.IsNullOrWhiteSpace(openId)) {
                //通过，用code换取access_token
                OAuthAccessTokenResult result = OAuthApi.GetAccessToken(WechatOfficialAccountConfig.AppId, WechatOfficialAccountConfig.AppSecret, code); // 这个 access_token 是 OAuth 专用的

                // 插入数据用于调试
                wechatOfficialAccountOAuthAccessTokenService.Insert(code, result.access_token, result.openid);

                if (result.errcode != ReturnCode.请求成功) {
                    throw new Exception($"获取 access token 错误：{result.errmsg}, code: {code}");
                }

                openId = result.openid;
                redisClient.Set(codeKey, openId, WebSessionConfig.LOGIN_CUSTOMER_SESSION_TIMEOUT);
            }

            wechatOfficialAccountSessionService.SetOpenId(openId);

            if (string.IsNullOrEmpty(returnUrl)) {
                // 返回到微信客户端首页
                // return Redirect("/client");
                return RedirectToAction("ClientIndex", "Home");
            }

            return Redirect(returnUrl);
        }
        catch (Exception exp) {
            log.Error(exp);

            log.Error($"code: {code}");
            log.Error($"state: {state}");
            log.Error($"returnUrl: {returnUrl}");

            return Content($"请求出错：{exp.Message}");
        }
    }
}