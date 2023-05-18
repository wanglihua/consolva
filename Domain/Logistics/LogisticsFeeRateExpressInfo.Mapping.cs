using NHibernate;
using NHibernate.Mapping.ByCode;
using shunda.Core;

namespace shunda.Domain.Logistics;

public class LogisticsFeeRateExpressInfoMapping : EntityMapping<LogisticsFeeRateExpressInfo> {
    public LogisticsFeeRateExpressInfoMapping() {
        Table("tbl_logistics_fee_rate_express");

        ManyToOne(
            x => x.LogisticsMode,
            map => {
                map.Class(typeof(LogisticsModeInfo));
                map.Column("logistics_mode_id");
                map.NotNullable(true);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        Property(
            x => x.DeliverPrice,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("deliver_price");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.StartWeight,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("start_weight");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.StartPrice,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("start_price");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.StepWeight,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("step_weight");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.StepPrice,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("step_price");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.StartFreeMinWeight,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("start_free_min_weight");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.WeightBreakStart,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("weight_break_start");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.WeightBreakDiscount,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("weight_break_discount");
                map.NotNullable(false);
            }
        );
    }
}