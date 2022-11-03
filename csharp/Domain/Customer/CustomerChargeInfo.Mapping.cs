using NHibernate;
using NHibernate.Mapping.ByCode;
using shunda.Core;
using shunda.Domain.Customer.Domain.Customer;
using shunda.Domain.Pass;

namespace shunda.Domain.Customer;

public class CustomerChargeInfoMapping : EntityMapping<CustomerChargeInfo>
{
    public CustomerChargeInfoMapping()
    {
        Table("tbl_customer_charge");

        Property(
            x => x.ChargeType,
            map => {
                map.Type<CustomerChargeTypeHibernateType>();
                map.Column("charge_type_id");
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
            x => x.Amount,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("amount");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Award,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("award");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.OutTradeNo,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("out_trade_no");
                map.Length(50);
                map.NotNullable(false);
            }
        );

        Property(
            x => x.PayPalPaymentId,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("paypal_payment_id");
                map.Length(50);
                map.NotNullable(false);
            }
        );

        Property(
            x => x.ManualChargeReason,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("manual_charge_reason");
                map.Length(300);
                map.NotNullable(false);
            }
        );

        Property(
            x => x.Status,
            map => {
                map.Type<CustomerChargeStatusHibernateType>();
                map.Column("status_id");
                map.NotNullable(true);
            }
        );

        ManyToOne(
            x => x.User,
            map => {
                map.Class(typeof(UserInfo));
                map.Column("user_id");
                map.NotNullable(false);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );
    }
}