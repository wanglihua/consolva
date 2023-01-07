using NHibernate;
using shunda.Core;

namespace shunda.Domain.Logistics;

public class LogisticsInsuranceInfoMapping : EntityMapping<LogisticsInsuranceInfo> {
    public LogisticsInsuranceInfoMapping() {
        Table("tbl_logistics_insurance");
        Mutable(false);

        Property(
            x => x.Name,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("name");
                map.Length(50);
                map.NotNullable(true);
            }
        );

        Property(
            x => x.CalcBaseType,
            map => {
                map.Type<FeeCalcBaseTypeHibernateType>();
                map.Column("calc_base_type_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Type,
            map => {
                map.Type<LogisticsInsuranceTypeHibernateType>();
                map.Column("type_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.FeeRate,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("fee_rate");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.OrderNum,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("order_num");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Status,
            map => {
                map.Type<LogisticsInsuranceStatusHibernateType>();
                map.Column("status_id");
                map.NotNullable(true);
            }
        );
    }
}