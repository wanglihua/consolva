using NHibernate;
using shunda.Core;

namespace shunda.Domain.Fee;

public class FeeRefundReasonInfoMapping : EntityMapping<FeeRefundReasonInfo>
{
    public FeeRefundReasonInfoMapping()
    {
        Table("tbl_fee_refund_reason");

        Property(
            x => x.Content,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("content");
                map.Length(300);
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Status,
            map => {
                map.Type<FeeRefundReasonStatusHibernateType>();
                map.Column("status_id");
                map.NotNullable(true);
            }
        );
    }
}