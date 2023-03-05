using NHibernate;
using NHibernate.Mapping.ByCode;
using shunda.Core;

namespace shunda.Domain.Coupon;

public class CouponLogInfoMapping : EntityNoUpdateMapping<CouponLogInfo> {
    public CouponLogInfoMapping() {
        Table("tbl_coupon_log");

        ManyToOne(
            x => x.Coupon,
            map => {
                map.Class(typeof(CouponInfo));
                map.Column("coupon_id");
                map.NotNullable(true);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        Property(
            x => x.Type,
            map => {
                map.Type<CouponLogTypeHibernateType>();
                map.Column("type_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Description,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("description");
                map.NotNullable(true);
            }
        );
    }
}