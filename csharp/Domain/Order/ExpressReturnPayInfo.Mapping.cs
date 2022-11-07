using NHibernate;
using NHibernate.Mapping.ByCode;
using shunda.Core;
using shunda.Domain.Customer;
using shunda.Domain.Express;

namespace shunda.Domain.Order;

public class ExpressReturnPayInfoMapping : EntityMapping<ExpressReturnPayInfo>
{
    public ExpressReturnPayInfoMapping()
    {
        Table("tbl_express_return_pay");

        ManyToOne(
            x => x.Express,
            map => {
                map.Class(typeof(ExpressInfo));
                map.Column("claim_id");
                map.NotNullable(true);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
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
            x => x.PayType,
            map => {
                map.Type<PayTypeHibernateType>();
                map.Column("pay_type_id");
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
                map.NotNullable(false);
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
            x => x.Status,
            map => {
                map.Type<PayStatusHibernateType>();
                map.Column("status_id");
                map.NotNullable(true);
            }
        );
    }
}