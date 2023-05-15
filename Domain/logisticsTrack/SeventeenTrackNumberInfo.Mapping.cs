using shunda.Core;
using NHibernate;

namespace shunda.Domain.LogisticsTrack;

public class SeventeenTrackNumberInfoMapping : EntityMapping<SeventeenTrackNumberInfo> {
    public SeventeenTrackNumberInfoMapping() {
        Table("tbl_seventeen_track_number");

        Property(
            x => x.TrackingNo,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("tracking_no");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.CarrierKey,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("carrier_key");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.CarrierName,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("carrier_name");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Origin,
            map => {
                map.Type<SeventeenTrackNumberOriginHibernateType>();
                map.Column("origin");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.ErrorCode,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("error_code");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.ErrorMessage,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("error_message");
                map.NotNullable(true);
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
            x => x.Status,
            map => {
                map.Type<SeventeenTrackNumberStatusHibernateType>();
                map.Column("status_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.TrackStatus,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("track_status");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.TrackSubStatus,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("track_sub_status");
                map.NotNullable(true);
            }
        );
    }
}