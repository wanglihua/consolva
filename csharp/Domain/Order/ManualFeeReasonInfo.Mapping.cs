using NHibernate;
using shunda.Core;

namespace shunda.Domain.Order;


public class ManualFeeReasonInfoMapping : EntityMapping<ManualFeeReasonInfo>
{
    public ManualFeeReasonInfoMapping()
    {
        Table("tbl_manual_fee_reason");

        Property(
            x => x.FeeType,
            map => {
                map.Type<ManualFeeTypeHibernateType>();
                map.Column("fee_type_id");
                map.NotNullable(true);
            }
        );

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
                map.Type<ManualFeeReasonStatusHibernateType>();
                map.Column("status_id");
                map.NotNullable(true);
            }
        );
    }
}
