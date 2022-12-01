using NHibernate.Mapping.ByCode;
using shunda.Core;

namespace shunda.Domain.Logistics;

public class LogisticsModeGoodsTypeInfoMapping : EntityMapping<LogisticsModeGoodsTypeInfo>
{
    public LogisticsModeGoodsTypeInfoMapping()
    {
        Table("tbl_logistics_mode_goods_type");

        ManyToOne(
            x => x.LogisticsMode,
            map => {
                map.Class(typeof(LogisticsModeInfo));
                map.Column("logistics_mode_id");
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
    }
}