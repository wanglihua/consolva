using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace shunda.Domain.LogisticsOrder;

public class LogisticsOrderCodeCustomerInfoMapping : ClassMapping<LogisticsOrderCodeCustomerInfo>
{
    public LogisticsOrderCodeCustomerInfoMapping()
    {
        Lazy(true);
        DynamicInsert(false);
        DynamicUpdate(true);
        Cache(x => x.Usage(CacheUsage.ReadWrite));

        Table("tbl_logistics_order_code_customer");

        Id(
            x => x.CustomerId,
            map => {
                map.Column("customer_id");
                map.Generator(Generators.Assigned);
            }
        );

        Property(
            x => x.Seq,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("seq");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.CreateTime,
            map => {
                map.Type(NHibernateUtil.DateTime);
                map.Column("create_time");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.UpdateTime,
            map => {
                map.Type(NHibernateUtil.DateTime);
                map.Column("update_time");
                map.NotNullable(true);
            }
        );
    }
}