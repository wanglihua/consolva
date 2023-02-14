using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace shunda.Domain.Client;

public class CustomerMessageViewRecordInfoMapping : ClassMapping<CustomerMessageViewRecordInfo> {
    public CustomerMessageViewRecordInfoMapping() {
        Table("tbl_customer_message_view_record");
        Lazy(true);
        DynamicInsert(false);
        DynamicUpdate(true);
        Cache(x => x.Usage(CacheUsage.ReadWrite));

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
    }
}