using shunda.Core;
using NHibernate;
using NHibernate.Mapping.ByCode;
using shunda.Domain.Customer;
using shunda.Domain.LogisticsOrder;

namespace shunda.Domain.Message;

public class CustomerMessageInfoMapping : EntityMapping<CustomerMessageInfo>
{
    public CustomerMessageInfoMapping()
    {
        Table("tbl_customer_message");

        Property(
            x => x.SendType,
            map => {
                map.Type<CustomerMessageSendTypeHibernateType>();
                map.Column("send_type_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Type,
            map => {
                map.Type<CustomerMessageTypeHibernateType>();
                map.Column("type_id");
                map.NotNullable(true);
            }
        );

        ManyToOne(
            x => x.Customer,
            map => {
                map.Class(typeof(CustomerInfo));
                map.Column("customer_id");
                map.NotNullable(true);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        ManyToOne(
            x => x.Order,
            map => {
                map.Class(typeof(LogisticsOrderInfo));
                map.Column("order_id");
                map.NotNullable(false);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
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

        Property(
            x => x.Status,
            map => {
                map.Type<CustomerMessageStatusHibernateType>();
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
            x => x.SendTime,
            map => {
                map.Type(NHibernateUtil.DateTime);
                map.Column("send_time");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Sender,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("sender");
                map.NotNullable(true);
            }
        );
    }
}