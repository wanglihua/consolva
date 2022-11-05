using NHibernate.Mapping.ByCode;
using shunda.Core;
using shunda.Domain.Express;

namespace shunda.Domain.Order;

public class LogisticsOrderExpressInfoMapping : EntityMapping<LogisticsOrderExpressInfo>
{
    public LogisticsOrderExpressInfoMapping()
    {
        Table("tbl_logistics_order_express");

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