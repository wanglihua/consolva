using shunda.Core;
using NHibernate;
using NHibernate.Mapping.ByCode;
using shunda.Domain.Customer;
using shunda.Domain.Order;
using shunda.Domain.Pass;

namespace shunda.Domain.Fee;

public class ManualFeeApplyInfoMapping : EntityMapping<ManualFeeApplyInfo>
{
    public ManualFeeApplyInfoMapping()
    {
        Table("tbl_manual_fee_apply");

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
            x => x.Type,
            map => {
                map.Type<LogisticsOrderFeeTypeHibernateType>();
                map.Column("type_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Fee,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("fee");
                map.NotNullable(true);
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

        ManyToOne(
            x => x.ManualReason,
            map => {
                map.Class(typeof(ManualFeeReasonInfo));
                map.Column("manual_reason_id");
                map.NotNullable(true);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        Property(
            x => x.ManualReasonContent,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("manual_reason_content");
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
                map.Type<ManualFeeApplyStatusHibernateType>();
                map.Column("status_id");
                map.NotNullable(true);
            }
        );

    }
}