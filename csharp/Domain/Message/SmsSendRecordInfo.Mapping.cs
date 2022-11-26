using NHibernate;
using shunda.Core;

namespace shunda.Domain.Message;

public class SmsSendRecordInfoMapping : EntityNoUpdateMapping<SmsSendRecordInfo>
{
    public SmsSendRecordInfoMapping()
    {
        Table("tbl_sms_send_record");

        Property(
            x => x.Mobile,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("mobile");
                map.NotNullable(true);
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
            x => x.SendMsgId,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("send_msg_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.SendCode,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("send_code");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.SendError,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("send_error");
                map.NotNullable(true);
            }
        );
    }
}