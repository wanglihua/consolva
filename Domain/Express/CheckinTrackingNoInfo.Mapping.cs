using NHibernate;
using shunda.Core;

namespace shunda.Domain.Express;

public class CheckinTrackingNoInfoMapping : EntityMapping<CheckinTrackingNoInfo>
{
    public CheckinTrackingNoInfoMapping()
    {
        Table("tbl_checkin_tracking_no");

        Property(
            x => x.Year,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("year");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Month,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("month");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Seq,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("seq");
                map.NotNullable(true);
            }
        );

    }
}