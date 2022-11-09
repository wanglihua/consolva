using NHibernate;
using NHibernate.Mapping.ByCode;
using shunda.Core;
using shunda.Domain.Customer;
using shunda.Domain.Order;

namespace shunda.Domain.Fee;

    public class FeeRefundInfoMapping : EntityMapping<FeeRefundInfo>
    {
        public FeeRefundInfoMapping()
        {
            Table("tbl_fee_refund");

            ManyToOne(
                x => x.Order,
                map => {
                    map.Class(typeof(LogisticsOrderInfo));
                    map.Column("order_id");
                    map.NotNullable(true);
                    map.Fetch(FetchKind.Select);
                    map.Cascade(Cascade.None);
                }
            );

            ManyToOne(
                x => x.Customer,
                map => {
                    map.Class(typeof(CustomerInfo));
                    map.Column("customer_id");
                    map.NotNullable(true);
                    map.Fetch(FetchKind.Select);
                    map.Cascade(Cascade.None);
                }
            );

            Property(
                x => x.Type,
                map => {
                    map.Type<FeeRefundTypeHibernateType>();
                    map.Column("type_id");
                    map.NotNullable(true);
                }
            );

            ManyToOne(
                x => x.Reason,
                map => {
                    map.Class(typeof(FeeRefundReasonInfo));
                    map.Column("reason_id");
                    map.NotNullable(true);
                    map.Fetch(FetchKind.Select);
                    map.Cascade(Cascade.None);
                }
            );

            Property(
                x => x.ReasonContent,
                map => {
                    map.Type(NHibernateUtil.String);
                    map.Column("reason");
                    map.Length(300);
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
                x => x.PayType,
                map => {
                    map.Type<FeeRefundPayTypeHibernateType>();
                    map.Column("pay_type_id");
                    map.NotNullable(true);
                }
            );

            Property(
                x => x.Status,
                map => {
                    map.Type<FeeRefundStatusHibernateType>();
                    map.Column("status_id");
                    map.NotNullable(true);
                }
            );

            ManyToOne(
                x => x.Apply,
                map => {
                    map.Class(typeof(FeeRefundApplyInfo));
                    map.Column("apply_id");
                    map.NotNullable(true);
                    map.Fetch(FetchKind.Select);
                    map.Cascade(Cascade.None);
                }
            );
        }
    }
