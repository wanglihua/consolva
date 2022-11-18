using NHibernate;
using NHibernate.Mapping.ByCode;
using shunda.Core;
using shunda.Domain.Fee;
using shunda.Domain.LogisticsOrder;

namespace shunda.Domain.Customer;

public class CustomerScoreLogInfoMapping : EntityMapping<CustomerScoreLogInfo>
{
    public CustomerScoreLogInfoMapping()
    {
        Table("tbl_customer_score_log");

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
            x => x.ScoreType,
            map => {
                map.Type<CustomerScoreTypeHibernateType>();
                map.Column("score_type_id");
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
            x => x.Amount,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("amount");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.IsValidScore,
            map => {
                map.Type(NHibernateUtil.Boolean);
                map.Column("is_valid_score");
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
            x => x.LogisticsOrderPay,
            map => {
                map.Class(typeof(LogisticsOrderPayInfo));
                map.Column("logistics_order_pay_id");
                map.NotNullable(false);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        ManyToOne(
            x => x.LogisticsOrder,
            map => {
                map.Class(typeof(LogisticsOrderInfo));
                map.Column("logistics_order_id");
                map.NotNullable(false);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        Property(
            x => x.LogisticsOrderCode,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("logistics_order_code");
                map.Length(100);
                map.NotNullable(false);
            }
        );

        ManyToOne(
            x => x.LogisticsOrderCustomer,
            map => {
                map.Class(typeof(CustomerInfo));
                map.Column("logistics_order_customer_id");
                map.NotNullable(false);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );
    }
}