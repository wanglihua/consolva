using NHibernate;
using NHibernate.Mapping.ByCode;
using shunda.Core;
using shunda.Domain.Pass;

namespace shunda.Domain.Customer;

public class CustomerSuperVipDiscountInfoMapping : EntityMapping<CustomerSuperVipDiscountInfo>
{
    public CustomerSuperVipDiscountInfoMapping()
    {
        Table("tbl_customer_super_vip_discount");

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
            x => x.Discount,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("discount");
                map.NotNullable(true);
            }
        );

        ManyToOne(
            x => x.User,
            map => {
                map.Class(typeof(UserInfo));
                map.Column("user_id");
                map.NotNullable(true);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        Property(
            x => x.Description,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("description");
                map.Length(500);
                map.NotNullable(false);
            }
        );
    }
}