using shunda.Core;
using NHibernate;
using NHibernate.Mapping.ByCode;

namespace shunda.Domain.Customer;

public class CustomerLoginLogInfoMapping : EntityNoCreateNoUpdateMapping<CustomerLoginLogInfo>
{
    public CustomerLoginLogInfoMapping()
    {
        Table("tbl_customer_login_log");

        Property(
            x => x.Stage,
            map => {
                map.Type<CustomerLoginStageHibernateType>();
                map.Column("stage_id");
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
            x => x.OpenId,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("open_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.SessionId,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("session_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.IpAddress,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("ip_address");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.UserAgent,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("user_agent");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.LoginTime,
            map => {
                map.Type(NHibernateUtil.DateTime);
                map.Column("login_time");
                map.NotNullable(true);
            }
        );

    }
}