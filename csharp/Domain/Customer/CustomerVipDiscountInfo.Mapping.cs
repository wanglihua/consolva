using NHibernate;
using NHibernate.Mapping.ByCode;
using shunda.Core;
using shunda.Domain.Pass;

namespace shunda.Domain.Customer;

public class CustomerVipDiscountInfoMapping : EntityMapping<CustomerVipDiscountInfo>
{
    public CustomerVipDiscountInfoMapping()
    {
        Table("tbl_customer_vip_discount");

        Property(
            x => x.Rank,
            map => {
                map.Type<CustomerVipRankHibernateType>();
                map.Column("rank_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Score,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("score");
                map.NotNullable(true);
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
    }
}