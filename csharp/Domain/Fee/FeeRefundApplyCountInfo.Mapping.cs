using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace shunda.Domain.Fee;

public class FeeRefundApplyCountInfoMapping : ClassMapping<FeeRefundApplyCountInfo>
{
    public FeeRefundApplyCountInfoMapping()
    {
        Mutable(false);

        Cache(x => x.Usage(CacheUsage.ReadOnly));

        Table("view_fee_refund_apply_count");

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