using NHibernate;
using shunda.Core;

namespace shunda.Domain.LogisticsOrder;

public class LogisticsOrderPackConfigInfoMapping : EntityMapping<LogisticsOrderPackConfigInfo> {
    public LogisticsOrderPackConfigInfoMapping() {
        Table("tbl_logistics_order_pack_config");

        Property(
            x => x.SortType,
            map => {
                map.Type<LogisticsOrderPackSortTypeHibernateType>();
                map.Column("sort_type_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.DisplayType,
            map => {
                map.Type<LogisticsOrderPackDisplayTypeHibernateType>();
                map.Column("display_type_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.PackDelayMinutes,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("pack_delay_minutes");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Label1015Enabled,
            map => {
                map.Type(NHibernateUtil.Boolean);
                map.Column("label1015_enabled");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Label1015PrintCount,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("label1015_print_count");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Label1005Enabled,
            map => {
                map.Type(NHibernateUtil.Boolean);
                map.Column("label1005_enabled");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Label1005PrintCount,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("label1005_print_count");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Label0505Enabled,
            map => {
                map.Type(NHibernateUtil.Boolean);
                map.Column("label0505_enabled");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Label0505PrintCount,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("label0505_print_count");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.TrackingNoManualInputEnabled,
            map => {
                map.Type(NHibernateUtil.Boolean);
                map.Column("tracking_no_manual_input_enabled");
                map.NotNullable(true);
            }
        );

    }
}