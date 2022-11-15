using shunda.Core;
using NHibernate;
using NHibernate.Mapping.ByCode;
using shunda.Domain.Customer;
using shunda.Domain.Order;
using shunda.Domain.Pass;

namespace shunda.Domain.Fee;

public class FeeWriteOffApplyInfoMapping : EntityMapping<FeeWriteOffApplyInfo>
{
    public FeeWriteOffApplyInfoMapping()
    {
        Table("tbl_fee_write_off_apply");

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

        Property(
            x => x.Amount,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("amount");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Description,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("description");
                map.NotNullable(false);
            }
        );

        ManyToOne(
            x => x.WriteOffReason,
            map => {
                map.Class(typeof(ManualFeeReasonInfo));
                map.Column("write_off_reason_id");
                map.NotNullable(true);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
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

        ManyToOne(
            x => x.ApplyUser,
            map => {
                map.Class(typeof(UserInfo));
                map.Column("apply_user_id");
                map.NotNullable(true);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        ManyToOne(
            x => x.ApproveUser,
            map => {
                map.Class(typeof(UserInfo));
                map.Column("approve_user_id");
                map.NotNullable(false);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        Property(
            x => x.Status,
            map => {
                map.Type<FeeWriteOffApplyStatusHibernateType>();
                map.Column("status_id");
                map.NotNullable(true);
            }
        );

    }
}