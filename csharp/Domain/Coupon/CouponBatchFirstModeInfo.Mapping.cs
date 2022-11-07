using NHibernate.Mapping.ByCode;
using shunda.Core;
using shunda.Domain.Logistics;

namespace shunda.Domain.Coupon;

public class CouponBatchFirstModeInfoMapping : EntityMapping<CouponBatchFirstModeInfo>
{
    public CouponBatchFirstModeInfoMapping()
    {
        Table("tbl_coupon_batch_first_mode");

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

        ManyToOne(
            x => x.FirstMode,
            map => {
                map.Class(typeof(LogisticsModeInfo));
                map.Column("first_mode_id");
                map.NotNullable(true);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );
    }
}