using NHibernate;
using shunda.Core;

namespace shunda.Domain.Wechat;

public class WechatCallbackInfoMapping : EntityNoUpdateMapping<WechatCallbackInfo> {
    public WechatCallbackInfoMapping() {
        Table("tbl_wechat_callback");

        Property(
            x => x.SystemType,
            map => {
                map.Type<WechatSystemTypeHibernateType>();
                map.Column("system_type_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Method,
            map => {
                map.Type<WechatCallbackHttpMethodHibernateType>();
                map.Column("method_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Params,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("params");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.RequestBody,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("request_body");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Result,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("result");
                map.NotNullable(true);
            }
        );
    }
}