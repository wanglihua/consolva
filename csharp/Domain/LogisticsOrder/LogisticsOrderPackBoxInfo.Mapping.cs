using shunda.Core;
using NHibernate;
using NHibernate.Mapping.ByCode;

namespace shunda.Domain.LogisticsOrder;

public class LogisticsOrderPackBoxInfoMapping : EntityMapping<LogisticsOrderPackBoxInfo>
{
    public LogisticsOrderPackBoxInfoMapping()
    {
        Table("tbl_logistics_order_pack_box");

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

        Property(
            x => x.BoxNum,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("box_num");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.ShipTime,
            map => {
                map.Type(NHibernateUtil.DateTime);
                map.Column("ship_time");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.DeliverTime,
            map => {
                map.Type(NHibernateUtil.DateTime);
                map.Column("deliver_time");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.Status,
            map => {
                map.Type<LogisticsOrderPackBoxStatusHibernateType>();
                map.Column("status_id");
                map.NotNullable(true);
            }
        );

    }
}