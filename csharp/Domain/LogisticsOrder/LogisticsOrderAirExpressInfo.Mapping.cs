using shunda.Core;
using NHibernate;
using NHibernate.Mapping.ByCode;
using shunda.Domain.Logistics;
using shunda.Domain.Pass;

namespace shunda.Domain.LogisticsOrder;

public class LogisticsOrderAirExpressInfoMapping : EntityMapping<LogisticsOrderAirExpressInfo>
{
    public LogisticsOrderAirExpressInfoMapping()
    {
        Table("tbl_logistics_order_air_express");

        ManyToOne(
            x => x.Order,
            map => {
                map.Class(typeof(LogisticsOrderInfo));
                map.Column("order_id");
                map.NotNullable(false);
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

        ManyToOne(
            x => x.LogisticsMode,
            map => {
                map.Class(typeof(LogisticsModeInfo));
                map.Column("logistics_mode_id");
                map.NotNullable(false);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        Property(
            x => x.TrackingNo,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("tracking_no");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.ShippingWeight,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("shipping_weight");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.EstimateFee,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("estimate_fee");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.ShippingFee,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("shipping_fee");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.Channel,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("channel");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.Memo,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("memo");
                map.NotNullable(false);
            }
        );

        ManyToOne(
            x => x.ImportUser,
            map => {
                map.Class(typeof(UserInfo));
                map.Column("import_user_id");
                map.NotNullable(false);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        Property(
            x => x.ImportTime,
            map => {
                map.Type(NHibernateUtil.DateTime);
                map.Column("import_time");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.DeliveredTime,
            map => {
                map.Type(NHibernateUtil.DateTime);
                map.Column("delivered_time");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.Status,
            map => {
                map.Type<LogisticsOrderAirExpressStatusHibernateType>();
                map.Column("status_id");
                map.NotNullable(true);
            }
        );

    }
}