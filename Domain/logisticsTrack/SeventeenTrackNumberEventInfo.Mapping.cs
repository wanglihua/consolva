using shunda.Core;
using NHibernate;
using NHibernate.Mapping.ByCode;

namespace shunda.Domain.LogisticsTrack;

public class SeventeenTrackNumberEventInfoMapping : EntityMapping<SeventeenTrackNumberEventInfo> {
    public SeventeenTrackNumberEventInfoMapping() {
        Table("tbl_seventeen_track_number_event");

        ManyToOne(
            x => x.TrackNumber,
            map => {
                map.Class(typeof(SeventeenTrackNumberInfo));
                map.Column("track_number_id");
                map.NotNullable(true);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        Property(
            x => x.EventTime,
            map => {
                map.Type(NHibernateUtil.DateTime);
                map.Column("event_time");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.EventDesc,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("event_desc");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Stage,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("stage");
                map.NotNullable(true);
            }
        );

    }
}