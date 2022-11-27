using NHibernate;
using shunda.Core;

namespace shunda.Domain.Message;

public class CustomerMessageRealTimeSendSettingInfoMapping : EntityMapping<CustomerMessageRealTimeSendSettingInfo>
{
    public CustomerMessageRealTimeSendSettingInfoMapping()
    {
        Table("tbl_customer_message_real_time_send_setting");

        Property(
            x => x.Type,
            map => {
                map.Type<CustomerMessageTypeHibernateType>();
                map.Column("type_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.WechatMessageTemplate1,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("wechat_message_template1");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.WechatMessageTemplate2,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("wechat_message_template2");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.WechatMessageTemplate3,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("wechat_message_template3");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.SmsMessageTemplate,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("sms_message_template");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Status,
            map => {
                map.Type<CustomerMessageSettingStatusHibernateType>();
                map.Column("status_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.SmsSend,
            map => {
                map.Type(NHibernateUtil.Boolean);
                map.Column("sms_send");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.WechatSend,
            map => {
                map.Type(NHibernateUtil.Boolean);
                map.Column("wechat_send");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Placeholders,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("placeholders");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.ClientCustomizable,
            map => {
                map.Type(NHibernateUtil.Boolean);
                map.Column("client_customizable");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.ClientTitle,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("client_title");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.ClientDesc,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("client_desc");
                map.NotNullable(true);
            }
        );

    }
}