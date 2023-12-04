using NHibernate;
using NHibernate.Mapping.ByCode;
using shunda.Core;

namespace shunda.Domain.Express;

public class CheckinTrackingNoExpressInfoMapping : EntityMapping<CheckinTrackingNoExpressInfo> {
    public CheckinTrackingNoExpressInfoMapping() {
        Table("tbl_checkin_tracking_no_express");

        Property(
            x => x.CheckinTrackingNo,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("checkin_tracking_no");
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
            x => x.Express,
            map => {
                map.Class(typeof(ExpressInfo));
                map.Column("express_id");
                map.NotNullable(true);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        ManyToOne(
            x => x.SizeWeight,
            map => {
                map.Class(typeof(ExpressSizeWeightInfo));
                map.Column("size_weight_id");
                map.NotNullable(false);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        Property(
            x => x.Status,
            map => {
                map.Type<AoPinTrackingNoExpressStatusHibernateType>();
                map.Column("status_id");
                map.NotNullable(true);
            }
        );
    }
}