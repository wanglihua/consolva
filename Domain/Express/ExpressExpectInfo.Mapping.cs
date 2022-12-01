using shunda.Core;
using NHibernate;
using NHibernate.Mapping.ByCode;
using shunda.Domain.Customer;

namespace shunda.Domain.Express;

public class ExpressExpectInfoMapping : EntityMapping<ExpressExpectInfo>
{
    public ExpressExpectInfoMapping()
    {
        Table("tbl_express_expect");

        Property(
            x => x.TrackingNo,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("tracking_no");
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

        ManyToOne(
            x => x.Customer,
            map => {
                map.Class(typeof(CustomerInfo));
                map.Column("customer_id");
                map.NotNullable(true);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        Property(
            x => x.LastEditTime,
            map => {
                map.Type(NHibernateUtil.DateTime);
                map.Column("last_edit_time");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Status,
            map => {
                map.Type<ExpressExpectStatusHibernateType>();
                map.Column("status_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.OverdueMsgSent,
            map => {
                map.Type(NHibernateUtil.DateTime);
                map.Column("overdue_msg_sent");
                map.NotNullable(false);
            }
        );

    }
}