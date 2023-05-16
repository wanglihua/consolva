using shunda.Core;
using NHibernate;
using NHibernate.Mapping.ByCode;
using shunda.Domain.Fee;
using shunda.Domain.Logistics;
using shunda.Domain.LogisticsTrack;

namespace shunda.Domain.LogisticsOrder;

public class LogisticsOrderFirstModeViewInfoMapping : EntityMapping<LogisticsOrderFirstModeViewInfo> {
    public LogisticsOrderFirstModeViewInfoMapping() {
        Table("view_logistics_order_first_mode");

        Mutable(false);

        Property(
            x => x.OrderId,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("order_id");
                map.NotNullable(true);
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

        Property(
            x => x.VolumeWeightDenominator,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("volume_weight_denominator");
                map.NotNullable(true);
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

        ManyToOne(
            x => x.SeventeenTrackNumber,
            map => {
                map.Class(typeof(SeventeenTrackNumberInfo));
                map.Column("seventeen_track_number_id");
                map.NotNullable(false);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        Property(
            x => x.FirstStatusTime,
            map => {
                map.Type(NHibernateUtil.DateTime);
                map.Column("first_status_time");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.LatestStatusTime,
            map => {
                map.Type(NHibernateUtil.DateTime);
                map.Column("latest_status_time");
                map.NotNullable(false);
            }
        );
        
        Property(
            x => x.StopDays,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("stop_days");
                map.NotNullable(false);
            }
        );        

        Property(
            x => x.TrackStatus,
            map => {
                map.Type<SeventeenTrackNumberTrackStatusHibernateType>();
                map.Column("track_status");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.TrackSubStatus,
            map => {
                map.Type<SeventeenTrackNumberTrackSubStatusHibernateType>();
                map.Column("track_sub_status");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.PayTime,
            map => {
                map.Type(NHibernateUtil.DateTime);
                map.Column("pay_time");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.PayStatus,
            map => {
                map.Type<PayStatusHibernateType>();
                map.Column("pay_status_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.TransferTime,
            map => {
                map.Type(NHibernateUtil.DateTime);
                map.Column("transfer_time");
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
    }
}