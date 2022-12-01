using NHibernate;
using shunda.Core;

namespace shunda.Domain.LogisticsOrder;

public class LogisticsOrderRemarkReasonInfoMapping : EntityMapping<LogisticsOrderRemarkReasonInfo>
{
    public LogisticsOrderRemarkReasonInfoMapping()
    {
        Table("tbl_logistics_order_remark_reason");

        Property(
            x => x.Reason,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("reason");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Description,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("description");
                map.NotNullable(true);
            }
        );

    }
}