using NHibernate;
using shunda.Core;

namespace shunda.Domain.Customer;

public class CustomerScoreRuleInfoMapping : EntityMapping<CustomerScoreRuleInfo>
{
    public CustomerScoreRuleInfoMapping()
    {
        Table("tbl_customer_score_rule");

        Property(
            x => x.Type,
            map => {
                map.Type<CustomerScoreTypeHibernateType>();
                map.Column("type_id");
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
            x => x.Score,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("score");
                map.NotNullable(true);
            }
        );
    }
}