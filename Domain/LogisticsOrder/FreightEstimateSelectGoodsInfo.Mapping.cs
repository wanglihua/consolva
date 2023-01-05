using NHibernate;
using shunda.Core;

namespace shunda.Domain.LogisticsOrder;

public class FreightEstimateSelectGoodsInfoMapping : EntityNoCreateNoUpdateMapping<FreightEstimateSelectGoodsInfo> {
    public FreightEstimateSelectGoodsInfoMapping() {
        Table("tbl_freight_estimate_select_goods");

        Property(
            x => x.CustomerId,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("customer_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Length,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("length");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Width,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("width");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Height,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("height");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Weight,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("weight");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Quantity,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("quantity");
                map.NotNullable(true);
            }
        );
    }
}