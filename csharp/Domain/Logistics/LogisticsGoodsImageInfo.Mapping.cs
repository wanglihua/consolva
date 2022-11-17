using shunda.Core;
using NHibernate;
using NHibernate.Mapping.ByCode;
using shunda.Domain.Express;

namespace shunda.Domain.Logistics;

public class LogisticsGoodsImageInfoMapping : EntityNoUpdateMapping<LogisticsGoodsImageInfo>
{
    public LogisticsGoodsImageInfoMapping()
    {
        Table("tbl_logistics_goods_image");

        ManyToOne(
            x => x.Goods,
            map => {
                map.Class(typeof(LogisticsGoodsInfo));
                map.Column("goods_id");
                map.NotNullable(true);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        Property(
            x => x.StorePath,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("store_path");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.StoreName,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("store_name");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.StoreLoc,
            map => {
                map.Type<ExpressPhotoStoreLocHibernateType>();
                map.Column("store_loc_id");
                map.NotNullable(true);
            }
        );

    }
}