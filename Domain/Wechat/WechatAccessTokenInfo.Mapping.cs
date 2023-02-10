using NHibernate;
using shunda.Core;

namespace shunda.Domain.Wechat;

public class WechatAccessTokenInfoMapping : EntityNoUpdateMapping<WechatAccessTokenInfo>
{
    public WechatAccessTokenInfoMapping()
    {
        Table("tbl_wechat_access_token");


        Property(
            x => x.SystemType,
            map => {
                map.Type<WechatSystemTypeHibernateType>();
                map.Column("system_type_id");
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
    }
}