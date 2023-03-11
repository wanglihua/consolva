using shunda.Core;
using NHibernate;

namespace shunda.Domain.Logistics;

public class LogisticsModeSettingInfoMapping : EntityMapping<LogisticsModeSettingInfo> {
    public LogisticsModeSettingInfoMapping() {
        Table("tbl_logistics_mode_setting");

        Property(
            x => x.VolumeWeightDenominator,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("volume_weight_denominator");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.HasFreePackageLimit,
            map => {
                map.Type(NHibernateUtil.Boolean);
                map.Column("has_free_package_limit");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.FreePackageLimit,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("free_package_limit");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.ExtraPackageFee,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("extra_package_fee");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.HasExtraPackageFeeCapAmount,
            map => {
                map.Type(NHibernateUtil.Boolean);
                map.Column("has_extra_package_fee_cap_amount");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.ExtraPackageFeeCapAmount,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("extra_package_fee_cap_amount");
                map.NotNullable(false);
            }
        );

    }
}