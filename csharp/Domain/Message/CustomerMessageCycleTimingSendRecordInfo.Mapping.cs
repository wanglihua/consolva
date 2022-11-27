using NHibernate;
using shunda.Core;

namespace shunda.Domain.Message;

public class CustomerMessageCycleTimingSendRecordInfoMapping : EntityNoUpdateMapping<CustomerMessageCycleTimingSendRecordInfo>
{
    public CustomerMessageCycleTimingSendRecordInfoMapping()
    {
        Table("tbl_customer_message_cycle_timing_send_record");

        Property(
            x => x.Type,
            map => {
                map.Type<CustomerMessageTypeHibernateType>();
                map.Column("type_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.SettingStatus,
            map => {
                map.Type<CustomerMessageSettingStatusHibernateType>();
                map.Column("setting_status_id");
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
    }
}