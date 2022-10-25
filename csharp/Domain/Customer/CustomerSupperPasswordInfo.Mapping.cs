using NHibernate;
using shunda.Core;

namespace shunda.Domain.Customer;

public class CustomerSupperPasswordInfoMapping : EntityMapping<CustomerSupperPasswordInfo>
{
    public CustomerSupperPasswordInfoMapping()
    {
        Table("tbl_customer_supper_password");

        Property(
            x => x.CustomerId,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("customer_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Password,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("password");
                map.Length(40);
                map.NotNullable(true);
            }
        );
    }
}