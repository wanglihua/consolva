using shunda.Core;
using NHibernate;

namespace shunda.Domain.Logistics;

public class LogisticsRepackTypeSettingInfoMapping : EntityMapping<LogisticsRepackTypeSettingInfo> {
    public LogisticsRepackTypeSettingInfoMapping() {
        Table("tbl_logistics_repack_type_setting");

        Property(
            x => x.Type,
            map => {
                map.Type<LogisticsRepackTypeHibernateType>();
                map.Column("type_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Title,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("title");
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
                map.Type<LogisticsRepackTypeStatusHibernateType>();
                map.Column("status_id");
                map.NotNullable(true);
            }
        );

    }
}