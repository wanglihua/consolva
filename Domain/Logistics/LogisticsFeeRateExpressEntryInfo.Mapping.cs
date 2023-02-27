using shunda.Core;
using NHibernate;
using NHibernate.Mapping.ByCode;

namespace shunda.Domain.Logistics;

public class LogisticsFeeRateExpressEntryInfoMapping : EntityMapping<LogisticsFeeRateExpressEntryInfo> {
    public LogisticsFeeRateExpressEntryInfoMapping() {
        Table("tbl_logistics_fee_rate_express_entry");

        ManyToOne(
            x => x.FeeRateExpress,
            map => {
                map.Class(typeof(LogisticsFeeRateExpressInfo));
                map.Column("fee_rate_express_id");
                map.NotNullable(true);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        Property(
            x => x.BeginWeight,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("begin_weight");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.EndWeight,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("end_weight");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.FeePerKilogram,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("fee_per_kilogram");
                map.NotNullable(true);
            }
        );
    }
}