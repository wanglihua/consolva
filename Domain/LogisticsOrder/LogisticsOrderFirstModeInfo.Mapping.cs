using shunda.Core;
using NHibernate;
using NHibernate.Mapping.ByCode;
using shunda.Domain.Logistics;

namespace shunda.Domain.LogisticsOrder;

public class LogisticsOrderFirstModeInfoMapping : EntityNoUpdateMapping<LogisticsOrderFirstModeInfo>
{
    public LogisticsOrderFirstModeInfoMapping()
    {
        Table("tbl_logistics_order_first_mode");

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
            x => x.OrderCode,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("order_code");
                map.NotNullable(true);
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

        Property(
            x => x.FirstType,
            map => {
                map.Type<LogisticsModeTypeHibernateType>();
                map.Column("first_type_id");
                map.NotNullable(true);
            }
        );

        ManyToOne(
            x => x.FirstMode,
            map => {
                map.Class(typeof(LogisticsModeInfo));
                map.Column("first_mode_id");
                map.NotNullable(true);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        Property(
            x => x.Channel,
            map => {
                map.Type<LogisticsModeChannelHibernateType>();
                map.Column("channel_id");
                map.NotNullable(true);
            }
        );
    }
}