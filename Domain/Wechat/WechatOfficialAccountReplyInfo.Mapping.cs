using shunda.Core;
using NHibernate;

namespace shunda.Domain.Wechat;

public class WechatOfficialAccountReplyInfoMapping : EntityMapping<WechatOfficialAccountReplyInfo> {
    public WechatOfficialAccountReplyInfoMapping() {
        Table("tbl_wechat_official_account_reply");

        Property(
            x => x.Type,
            map => {
                map.Type<WechatOfficialAccountReplyTypeHibernateType>();
                map.Column("type_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Content,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("content");
                map.NotNullable(true);
            }
        );

    }
}