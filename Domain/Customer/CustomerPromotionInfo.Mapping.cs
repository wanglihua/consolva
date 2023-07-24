using shunda.Core;
using NHibernate;
using NHibernate.Mapping.ByCode;

namespace shunda.Domain.Customer;

public class CustomerPromotionInfoMapping : EntityMapping<CustomerPromotionInfo> {
    public CustomerPromotionInfoMapping() {
        Table("tbl_customer_promotion");

        ManyToOne(
            x => x.Parent,
            map => {
                map.Class(typeof(CustomerInfo));
                map.Column("parent_id");
                map.NotNullable(true);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        ManyToOne(
            x => x.Child,
            map => {
                map.Class(typeof(CustomerInfo));
                map.Column("child_id");
                map.NotNullable(false);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        Property(
            x => x.OpenId,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("open_id");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.ChildPayAmount,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("child_pay_amount");
                map.NotNullable(false);
            }
        );

    }
}