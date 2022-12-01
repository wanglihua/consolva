using NHibernate;
using NHibernate.Mapping.ByCode;
using shunda.Core;

namespace shunda.Domain.Customer;

public class CustomerAccountEntryInfoMapping : EntityMapping<CustomerAccountEntryInfo>
{
    public CustomerAccountEntryInfoMapping()
    {
        Table("tbl_customer_account_entry");

        Property(
            x => x.ChangeType,
            map => {
                map.Type<CustomerAccountChangeTypeHibernateType>();
                map.Column("change_type_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Amount,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("amount");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.AccountBefore,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("account_before");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.AccountAfter,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("account_after");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Description,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("description");
                map.Length(300);
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
    }
}