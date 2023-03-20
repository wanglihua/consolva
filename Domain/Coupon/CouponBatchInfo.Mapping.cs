using NHibernate;
using shunda.Core;

namespace shunda.Domain.Coupon;

public class CouponBatchInfoMapping : EntityMapping<CouponBatchInfo>
{
    public CouponBatchInfoMapping()
    {
        Table("tbl_coupon_batch");

        Property(
            x => x.Name,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("name");
                map.NotNullable(true);
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
            x => x.Type,
            map => {
                map.Type<CouponTypeHibernateType>();
                map.Column("type_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Expire,
            map => {
                map.Type(NHibernateUtil.DateTime);
                map.Column("expire");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.TotalQuantity,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("total_quantity");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.PickQuantity,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("pick_quantity");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.UseQuantity,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("use_quantity");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Rule,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("rule");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.RuleDesc,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("rule_desc");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.MaxQtyPerCustomer,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("max_qty_per_customer");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Status,
            map => {
                map.Type<CouponBatchStatusHibernateType>();
                map.Column("status_id");
                map.NotNullable(true);
            }
        );
    }
}