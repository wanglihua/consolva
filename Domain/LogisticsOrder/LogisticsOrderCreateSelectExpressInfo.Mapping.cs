using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace shunda.Domain.LogisticsOrder;

public class LogisticsOrderCreateSelectExpressInfoMapping : ClassMapping<LogisticsOrderCreateSelectExpressInfo>
{
    public LogisticsOrderCreateSelectExpressInfoMapping()
    {
        Lazy(true);
        DynamicInsert(false);
        DynamicUpdate(true);
        Cache(x => x.Usage(CacheUsage.ReadWrite));

        Table("tbl_logistics_order_create_select_express");

        ComposedId(ci => {
            ci.Property(
                x => x.CustomerId,
                map => {
                    map.Type(NHibernateUtil.Int32);
                    map.Column("customer_id");
                    map.NotNullable(true);
                }
            );

            ci.Property(
                x => x.ExpressId,
                map => {
                    map.Type(NHibernateUtil.Int32);
                    map.Column("express_id");
                    map.NotNullable(true);
                }
            );
        });
    }
}