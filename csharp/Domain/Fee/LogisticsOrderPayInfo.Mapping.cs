using NHibernate;
using NHibernate.Mapping.ByCode;
using shunda.Core;
using shunda.Domain.Customer;
using shunda.Domain.LogisticsOrder;
using shunda.Domain.Pass;

namespace shunda.Domain.Fee;

public class LogisticsOrderPayInfoMapping : EntityMapping<LogisticsOrderPayInfo>
{
    public LogisticsOrderPayInfoMapping()
    {
        Table("tbl_logistics_order_pay");

        ManyToOne(
            x => x.Order,
            map => {
                map.Class(typeof(LogisticsOrderInfo));
                map.Column("order_id");
                map.NotNullable(true);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        Property(
            x => x.OrderCode,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("order_code");
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
                map.Length(25);
                map.NotNullable(true);
            }
        );

        Property(
            x => x.PayPalPaymentId,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("paypal_payment_id");
                map.Length(50);
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
            x => x.Status,
            map => {
                map.Type<PayStatusHibernateType>();
                map.Column("status_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.WriteOffReasonId,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("write_off_reason_id");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.WriteOffReasonContent,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("write_off_reason_content");
                map.NotNullable(true);
            }
        );
    }
}