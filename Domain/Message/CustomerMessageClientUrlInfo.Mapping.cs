using shunda.Core;
using NHibernate;
using NHibernate.Mapping.ByCode;

namespace shunda.Domain.Message;

public class CustomerMessageClientUrlInfoMapping : EntityMapping<CustomerMessageClientUrlInfo>
{
    public CustomerMessageClientUrlInfoMapping()
    {
        Table("tbl_customer_message_client_url");

        Property(
            x => x.MessageType,
            map => {
                map.Type<CustomerMessageTypeHibernateType>();
                map.Column("message_type_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.TerminalType,
            map => {
                map.Type<TerminalTypeHibernateType>();
                map.Column("terminal_type_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.ClientUrl,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("client_url");
                map.NotNullable(true);
            }
        );

    }
}