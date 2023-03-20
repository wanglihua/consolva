using NHibernate;
using NHibernate.Mapping.ByCode;
using shunda.Core;
using shunda.Domain.Customer;
using shunda.Domain.LogisticsOrder;

namespace shunda.Domain.Coupon;

public class CouponInfoMapping : EntityMapping<CouponInfo>
{
    public CouponInfoMapping()
    {
        Table("tbl_coupon");

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
            x => x.Code,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("code");
                map.NotNullable(true);
            }
        );

        ManyToOne(
            x => x.Customer,
            map => {
                map.Class(typeof(CustomerInfo));
                map.Column("customer_id");
                map.NotNullable(true);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        ManyToOne(
            x => x.Order,
            map => {
                map.Class(typeof(LogisticsOrderInfo));
                map.Column("order_id");
                map.NotNullable(false);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        Property(
            x => x.OrderCode,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("order_code");
                map.NotNullable(true);
            }
        );        

        Property(
            x => x.ActualDiscountAmount,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("actual_discount_amount");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.PickTime,
            map => {
                map.Type(NHibernateUtil.DateTime);
                map.Column("pick_time");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.UseTime,
            map => {
                map.Type(NHibernateUtil.DateTime);
                map.Column("use_time");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.Expire,
            map => {
                map.Type(NHibernateUtil.DateTime);
                map.Column("expire");
                map.NotNullable(false);
            }
        );        

        Property(
            x => x.GetMethod,
            map => {
                map.Type<CouponGetMethodHibernateType>();
                map.Column("get_method_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Status,
            map => {
                map.Type<CouponStatusHibernateType>();
                map.Column("status_id");
                map.NotNullable(true);
            }
        );
    }
}