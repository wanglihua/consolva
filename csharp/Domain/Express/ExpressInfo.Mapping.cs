using shunda.Core;
using NHibernate;
using NHibernate.Mapping.ByCode;

namespace shunda.Domain.Express;

public class ExpressInfoMapping : EntityMapping<ExpressInfo>
{
    public ExpressInfoMapping()
    {
        Table("tbl_express");

        Property(
            x => x.TrackingNo,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("tracking_no");
                map.NotNullable(true);
            }
        );

    }
}