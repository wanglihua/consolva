using shunda.Core;
using NHibernate;
using NHibernate.Mapping.ByCode;

namespace shunda.Domain.Logistics;

public class LogisticsModeFeeRateInfoMapping : EntityMapping<LogisticsModeFeeRateInfo>
{
    public LogisticsModeFeeRateInfoMapping()
    {
        Table("tbl_logistics_mode_fee_rate");

        ManyToOne(
            x => x.Mode,
            map => {
                map.Class(typeof(LogisticsModeInfo));
                map.Column("mode_id");
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
            x => x.FeeRate,
            map => {
                map.Class(typeof(LogisticsFeeRateInfo));
                map.Column("fee_rate_id");
                map.NotNullable(true);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

    }
}