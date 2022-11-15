using NHibernate;
using NHibernate.Mapping.ByCode;
using shunda.Core;
using shunda.Domain.Customer;
using shunda.Domain.Order;
using shunda.Domain.Pass;

namespace shunda.Domain.Fee;

public class FeeRefundApplyInfoMapping : EntityMapping<FeeRefundApplyInfo>
{
    public FeeRefundApplyInfoMapping()
    {
        Table("tbl_fee_refund_apply");

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
                map.Type<FeeRefundApplyStatusHibernateType>();
                map.Column("status_id");
                map.NotNullable(true);
            }
        );

        ManyToOne(
            x => x.ApplyUser,
            map => {
                map.Class(typeof(UserInfo));
                map.Column("apply_user_id");
                map.NotNullable(true);
                map.Fetch(FetchKind.Select);
                map.Cascade(Cascade.None);
            }
        );

        Property(
            x => x.ApplyTime,
            map => {
                map.Type(NHibernateUtil.DateTime);
                map.Column("apply_time");
            }
        );

        Property(
            x => x.ApplyMemo,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("apply_memo");
                map.Length(300);
                map.NotNullable(true);
            }
        );

        ManyToOne(
            x => x.ApproveUser,
            map => {
                map.Class(typeof(UserInfo));
                map.Column("approve_user_id");
                map.NotNullable(false);
                map.Fetch(FetchKind.Select);
                map.Cascade(Cascade.None);
            }
        );

        Property(
            x => x.ApproveTime,
            map => {
                map.Type(NHibernateUtil.DateTime);
                map.Column("approve_time");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.ApproveMemo,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("approve_memo");
                map.Length(300);
                map.NotNullable(true);
            }
        );
    }
}