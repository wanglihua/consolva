using shunda.Core;
using NHibernate;

namespace shunda.Domain.Wechat;

public class WechatOfficialAccountReplyKeywordInfoMapping : EntityMapping<WechatOfficialAccountReplyKeywordInfo> {
    public WechatOfficialAccountReplyKeywordInfoMapping() {
        Table("tbl_wechat_official_account_reply_keyword");

        Property(
            x => x.Keyword,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("keyword");
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