using shunda.Core;
using NHibernate;
using NHibernate.Mapping.ByCode;
using shunda.Domain.Pass;

namespace shunda.Domain.Customer;

public class CustomerChargeInfoMapping : EntityMapping<CustomerChargeInfo> {
    public CustomerChargeInfoMapping() {
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
                map.NotNullable(true);
            }
        );

        Property(
            x => x.PaypalPaymentId,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("paypal_payment_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.ManualChargeReason,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("manual_charge_reason");
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

        Property(
            x => x.Description,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("description");
                map.NotNullable(true);
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
    }
}