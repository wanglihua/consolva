using NHibernate;
using NHibernate.Mapping.ByCode;
using shunda.Core;
using shunda.Domain.Pass;

namespace shunda.Domain.LogisticsOrder;

public class LogisticsOrderPackDataInfoMapping : EntityMapping<LogisticsOrderPackDataInfo> {
    public LogisticsOrderPackDataInfoMapping() {
        Table("tbl_logistics_order_pack_data");

        ManyToOne(
            x => x.Order,
            map => {
                map.Class(typeof(LogisticsOrderInfo));
                map.Column("order_id");
                map.NotNullable(true);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        Property(
            x => x.TotalActualWeight,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("total_actual_weight");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.TotalVolumeWeight,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("total_volume_weight");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.TotalVolume,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("total_volume");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.MaxSideLength,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("max_side_length");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.MaxVolumeSinglePack,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("max_volume_single_pack");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.MaxWeightSinglePack,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("max_weight_single_pack");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.TotalPackageCount,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("total_package_count");
                map.NotNullable(true);
            }
        );

        ManyToOne(
            x => x.PackUser,
            map => {
                map.Class(typeof(UserInfo));
                map.Column("pack_user_id");
                map.NotNullable(true);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );
    }
}