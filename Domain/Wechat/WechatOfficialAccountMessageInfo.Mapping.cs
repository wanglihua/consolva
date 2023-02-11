using shunda.Core;
using NHibernate;
using NHibernate.Mapping.ByCode;
using shunda.Domain.Customer;
using shunda.Domain.Pass;

namespace shunda.Domain.Wechat;

public class WechatOfficialAccountMessageInfoMapping : EntityMapping<WechatOfficialAccountMessageInfo> {
    public WechatOfficialAccountMessageInfoMapping() {
        Table("tbl_wechat_official_account_message");

        Property(
            x => x.Direction,
            map => {
                map.Type<WechatOfficialAccountMessageDirectionHibernateType>();
                map.Column("direction_id");
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

        ManyToOne(
            x => x.Customer,
            map => {
                map.Class(typeof(CustomerInfo));
                map.Column("customer_id");
                map.NotNullable(false);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        ManyToOne(
            x => x.User,
            map => {
                map.Class(typeof(UserInfo));
                map.Column("user_id");
                map.NotNullable(false);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        Property(
            x => x.Message,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("message");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Status,
            map => {
                map.Type<WechatOfficialAccountMessageStatusHibernateType>();
                map.Column("status_id");
                map.NotNullable(true);
            }
        );
    }
}