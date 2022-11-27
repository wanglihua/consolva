using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using shunda.Domain.Customer;

namespace shunda.Domain.Message;

public class CustomerMessageViewRecordInfoMapping : ClassMapping<CustomerMessageViewRecordInfo>
{
    public CustomerMessageViewRecordInfoMapping()
    {
        Lazy(true);
        DynamicInsert(false);
        DynamicUpdate(true);
        Cache(x => x.Usage(CacheUsage.ReadWrite));

        Table("tbl_customer_message_view_record");

        Id(
            x => x.CustomerId,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("customer_id");
                map.Generator(Generators.Assigned);
            }
        );

        ManyToOne(
            x => x.LastViewMessage,
            map => {
                map.Class(typeof(CustomerMessageInfo));
                map.Column("last_view_message_id");
                map.NotNullable(true);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
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