using NHibernate;
using shunda.Core;

namespace shunda.Domain.Customer;

public class CustomerCheckinCodeInfoMapping : EntityNoCreateNoUpdateMapping<CustomerCheckinCodeInfo>
{
    public CustomerCheckinCodeInfoMapping()
    {
        Table("view_customer_checkin_code");
        Mutable(false); // 视图，不可变
        
        Property(
            x => x.CheckinCode,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("checkin_code");
                map.NotNullable(true);
            }
        );
    }
}