using NHibernate;
using shunda.Core;

namespace shunda.Domain.Customer;

public class CustomerCharacterInfoMapping : EntityMapping<CustomerCharacterInfo>
{
    public CustomerCharacterInfoMapping()
    {
        Table("tbl_customer_character");

        Property(
            x => x.Content,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("content");
                map.Length(300);
                map.NotNullable(true);
            }
        );
    }
}