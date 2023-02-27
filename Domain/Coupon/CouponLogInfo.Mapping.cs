using shunda.Core;
using NHibernate;
using NHibernate.Mapping.ByCode;
using shunda.Domain.Customer;
using shunda.Domain.Pass;

namespace shunda.Domain.Coupon;

public class CouponLogInfoMapping : EntityMapping<CouponLogInfo> {
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

        ManyToOne(
            x => x.Customer,
            map => {
                map.Class(typeof(CustomerInfo));
                map.Column("customer_id");
                map.NotNullable(false);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        ManyToOne(
            x => x.User,
            map => {
                map.Class(typeof(UserInfo));
                map.Column("user_id");
                map.NotNullable(false);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
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