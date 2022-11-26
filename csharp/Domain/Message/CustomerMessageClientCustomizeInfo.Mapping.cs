using NHibernate;
using shunda.Core;

namespace shunda.Domain.Message;

public class CustomerMessageClientCustomizeInfoMapping : EntityMapping<CustomerMessageClientCustomizeInfo>
{
    public CustomerMessageClientCustomizeInfoMapping()
    {
        Table("tbl_customer_message_client_customize");

        Property(
            x => x.CustomerId,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("customer_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.MessageType,
            map => {
                map.Type<CustomerMessageTypeHibernateType>();
                map.Column("type_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Send,
            map => {
                map.Type(NHibernateUtil.Boolean);
                map.Column("send");
                map.NotNullable(true);
            }
        );
    }
}