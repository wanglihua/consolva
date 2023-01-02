using NHibernate.Mapping.ByCode;
using shunda.Core;
using shunda.Domain.Express;

namespace shunda.Domain.LogisticsOrder;

public class LogisticsOrderPackExpressInfoMapping : EntityNoUpdateMapping<LogisticsOrderPackExpressInfo>
{
    public LogisticsOrderPackExpressInfoMapping()
    {
        Table("tbl_logistics_order_pack_express");

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

        ManyToOne(
            x => x.OrderPack,
            map => {
                map.Class(typeof(LogisticsOrderPackInfo));
                map.Column("order_pack_id");
                map.NotNullable(true);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        ManyToOne(
            x => x.Express,
            map => {
                map.Class(typeof(ExpressInfo));
                map.Column("express_id");
                map.NotNullable(true);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

    }
}