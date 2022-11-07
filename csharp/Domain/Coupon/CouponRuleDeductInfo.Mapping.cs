using NHibernate;
using NHibernate.Mapping.ByCode;
using shunda.Core;

namespace shunda.Domain.Coupon;

public class CouponRuleDeductInfoMapping : EntityMapping<CouponRuleDeductInfo>
{
    public CouponRuleDeductInfoMapping()
    {
        Table("tbl_coupon_rule_deduct");

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
            x => x.Amount,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("amount");
                map.NotNullable(true);
            }
        );
    }
}