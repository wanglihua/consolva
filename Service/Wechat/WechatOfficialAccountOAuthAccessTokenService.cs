using NLog;
using shunda.Core.Config;
using shunda.Core.Service;
using shunda.Domain.Wechat;
using ISession = NHibernate.ISession;

namespace shunda.Service.Wechat;

[Service(ServiceType.Scoped)]
public class WechatOfficialAccountOAuthAccessTokenService {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;

    public WechatOfficialAccountOAuthAccessTokenService(ISession session) {
        this.session = session;
    }

    public void Insert(string code, string accessToken, string openId) {
        if (WechatOfficialAccountConfig.LogOAuthAccessToken) {
            try {
                if (string.IsNullOrWhiteSpace(code)) {
                    code = string.Empty;
                }

                if (string.IsNullOrWhiteSpace(accessToken)) {
                    accessToken = string.Empty;
                }

                if (string.IsNullOrWhiteSpace(openId)) {
                    openId = string.Empty;
                }

                var token = new WechatOfficialAccountOAuthAccessTokenInfo {
                    Code = code,
                    AccessToken = accessToken,
                    OpenId = openId
                };

                session.Save(token);
                session.Flush();
            }
            catch (Exception exp) {
                log.Error(exp);
            }
        }
    }
}