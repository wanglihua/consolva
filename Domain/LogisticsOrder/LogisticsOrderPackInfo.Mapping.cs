using shunda.Core;
using NHibernate;
using NHibernate.Mapping.ByCode;
using shunda.Domain.Logistics;
using shunda.Domain.Pass;

namespace shunda.Domain.LogisticsOrder;

public class LogisticsOrderPackInfoMapping : EntityMapping<LogisticsOrderPackInfo> {
    public LogisticsOrderPackInfoMapping() {
        Table("tbl_logistics_order_pack");

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
            x => x.GoodsType,
            map => {
                map.Type<LogisticsGoodsTypeHibernateType>();
                map.Column("goods_type_id");
                map.NotNullable(true);
            }
        );

        ManyToOne(
            x => x.OrderFirstMode,
            map => {
                map.Class(typeof(LogisticsOrderFirstModeInfo));
                map.Column("order_first_mode_id");
                map.NotNullable(true);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
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
            x => x.PackageCount,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("package_count");
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

        Property(
            x => x.PackTime,
            map => {
                map.Type(NHibernateUtil.DateTime);
                map.Column("pack_time");
                map.NotNullable(true);
            }
        );
    }
}