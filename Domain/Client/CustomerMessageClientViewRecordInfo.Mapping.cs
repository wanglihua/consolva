using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace shunda.Domain.Client;

public class CustomerMessageClientViewRecordInfoMapping : ClassMapping<CustomerMessageClientViewRecordInfo>
{
    public CustomerMessageClientViewRecordInfoMapping()
    {
        Lazy(true);
        DynamicInsert(false);
        DynamicUpdate(true);
        Cache(x => x.Usage(CacheUsage.ReadWrite));

        Table("tbl_customer_message_client_view_record");

        Id(
            x => x.CustomerId,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("customer_id");
                map.Generator(Generators.Assigned);
            }
        );

        Property(
            x => x.LastViewMessageId,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("last_view_message_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.LastViewTime,
            map => {
                map.Type(NHibernateUtil.DateTime);
                map.Column("last_view_time");
                map.NotNullable(true);
            }
        );
    }
}