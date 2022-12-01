using NHibernate;
using shunda.Core;

namespace shunda.Domain.Customer;

public class ManualChargeReasonInfoMapping : EntityMapping<ManualChargeReasonInfo>
{
    public ManualChargeReasonInfoMapping()
    {
        Table("tbl_manual_charge_reason");

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
                map.Type<ManualChargeReasonStatusHibernateType>();
                map.Column("status_id");
                map.NotNullable(true);
            }
        );
    }
}