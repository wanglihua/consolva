using shunda.Core;
using NHibernate;
using NHibernate.Mapping.ByCode;
using shunda.Domain.Customer;
using shunda.Domain.Order;

namespace shunda.Domain.Fee;

public class LogisticsOrderFeeWriteOffApplyInfoMapping : EntityMapping<LogisticsOrderFeeWriteOffApplyInfo>
{
    public LogisticsOrderFeeWriteOffApplyInfoMapping()
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

        Property(
            x => x.ApplyUserId,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("apply_user_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.ApproveUserId,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("approve_user_id");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.Status,
            map => {
                map.Type<LogisticsOrderFeeWriteOffApplyStatusHibernateType>();
                map.Column("status_id");
                map.NotNullable(true);
            }
        );

    }
}