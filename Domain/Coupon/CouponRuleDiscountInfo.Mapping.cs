using NHibernate;
using NHibernate.Mapping.ByCode;
using shunda.Core;

namespace shunda.Domain.Coupon;

public class CouponRuleDiscountInfoMapping : EntityMapping<CouponRuleDiscountInfo>
{
    public CouponRuleDiscountInfoMapping()
    {
        Table("tbl_coupon_rule_discount");

        ManyToOne(
            x => x.Batch,
            map => {
                map.Class(typeof(CouponBatchInfo));
                map.Column("batch_id");
                map.NotNullable(true);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        Property(
            x => x.Rate,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("rate");
                map.NotNullable(true);
            }
        );
    }
}