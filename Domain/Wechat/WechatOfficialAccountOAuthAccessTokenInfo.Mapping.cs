using shunda.Core;
using NHibernate;

namespace shunda.Domain.Wechat;

public class WechatOfficialAccountOAuthAccessTokenInfoMapping : EntityNoUpdateMapping<WechatOfficialAccountOAuthAccessTokenInfo> {
    public WechatOfficialAccountOAuthAccessTokenInfoMapping() {
        Table("tbl_wechat_official_account_oauth_access_token");

        Property(
            x => x.Code,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("code");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.AccessToken,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("access_token");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.OpenId,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("open_id");
                map.NotNullable(true);
            }
        );

    }
}