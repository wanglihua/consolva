using NHibernate;
using NHibernate.Linq;
using NLog;
using Senparc.Weixin.MP.Containers;
using Senparc.Weixin.MP.Entities;
using shunda.Core.Config;
using shunda.Core.Service;
using shunda.Domain.Wechat;

namespace shunda.Service.Wechat.WechatOfficialAccount;

/// <summary>
/// 这里的实现是用数据库表来存储的，跟现在微信小程序的实现不太一样
/// </summary>
[Service(ServiceType.Scoped)]
public class WechatOfficialAccountAccessTokenService {
    private static Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;

    public WechatOfficialAccountAccessTokenService(ISession session) {
        this.session = session;
    }

    public string GetAccessToken() {
        DateTime now = DateTime.Now;

        // 获取最新的 access token
        WechatAccessTokenInfo? wechatAccessToken = session.Query<WechatAccessTokenInfo>()
            .OrderByDescending(e => e.Id)
            .Take(1)
            .SingleOrDefault();

        if (wechatAccessToken == null // 为空，一般程序开始的时候才会有这种情况
            || wechatAccessToken.Expire <= now) // 最新的已过期
        {
            if (wechatAccessToken != null) // 只是过期
            {
                if (now.Date.Day == 1 && now.Hour > 4 && now.Hour < 8) // 每月的 1日，4个小时内最多过期两次，最多也就执行2次了。
                {
                    int latestId = wechatAccessToken.Id;
                    var tokenList = session.Query<WechatAccessTokenInfo>()
                        .Where(e => e.Id < latestId) // 这里暂时用 < 
                        .ToList();
                    foreach (var token in tokenList) {
                        session.Delete(token);
                    }

                    session.Flush();
                }
            }

            AccessTokenResult tokenResult = AccessTokenContainer.GetAccessTokenResult(WeChatConfig.AppId, true);

            if (string.IsNullOrWhiteSpace(tokenResult.access_token)) {
                throw new Exception("获取token出错：" + tokenResult.errmsg);
            }

            log.Warn("get weixin access token. " + tokenResult.access_token);

            wechatAccessToken = new WechatAccessTokenInfo() {
                AccessToken = tokenResult.access_token,
                Expire = DateTime.Now + TimeSpan.FromSeconds(tokenResult.expires_in - 60), // 比实际失效提早个一会会
            };

            session.Save(wechatAccessToken);
            session.Flush();
        }

        return wechatAccessToken.AccessToken;
    }
}