using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace shunda.Domain.Fee;

public class LogisticsOrderRefundApplyCountInfoMapping : ClassMapping<LogisticsOrderRefundApplyCountInfo>
{
    public LogisticsOrderRefundApplyCountInfoMapping()
    {
        Mutable(false);

        Cache(x => x.Usage(CacheUsage.ReadOnly));

        Table("view_logistics_order_refund_apply_count");

        Id(
            x => x.OrderId,
            map => {
                map.Column("order_id");
                map.Generator(Generators.Assigned);
            }
        );

        Property(
            x => x.ApplyCount,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("apply_count");
            }
        );
    }
}