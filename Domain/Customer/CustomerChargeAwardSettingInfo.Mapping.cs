using shunda.Core;
using NHibernate;

namespace shunda.Domain.Customer;

public class CustomerChargeAwardSettingInfoMapping : EntityMapping<CustomerChargeAwardSettingInfo> {
    public CustomerChargeAwardSettingInfoMapping() {
        Table("tbl_customer_charge_award_setting");

        Property(
            x => x.AmountBegin,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("amount_begin");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.AmountEnd,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("amount_end");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Award,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("award");
                map.NotNullable(true);
            }
        );

    }
}