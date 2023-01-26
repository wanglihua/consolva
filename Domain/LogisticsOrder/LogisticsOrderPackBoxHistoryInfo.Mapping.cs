using NHibernate;
using NHibernate.Mapping.ByCode;
using shunda.Core;
using shunda.Domain.Pass;

namespace shunda.Domain.LogisticsOrder;

public class LogisticsOrderPackBoxHistoryInfoMapping : EntityNoUpdateMapping<LogisticsOrderPackBoxHistoryInfo> {
    public LogisticsOrderPackBoxHistoryInfoMapping() {
        Table("tbl_logistics_order_pack_box_history");

        Property(
            x => x.Type,
            map => {
                map.Type<LogisticsOrderPackBoxHistoryTypeHibernateType>();
                map.Column("type_id");
                map.NotNullable(true);
            }
        );

        ManyToOne(
            x => x.Order,
            map => {
                map.Class(typeof(LogisticsOrderInfo));
                map.Column("order_id");
                map.NotNullable(true);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        ManyToOne(
            x => x.PackBox,
            map => {
                map.Class(typeof(LogisticsOrderPackBoxInfo));
                map.Column("box_id");
                map.NotNullable(true);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        Property(
            x => x.BoxNum,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("box_num");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.StatusBefore,
            map => {
                map.Type<LogisticsOrderPackBoxStatusHibernateType>();
                map.Column("status_id_before");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.StatusAfter,
            map => {
                map.Type<LogisticsOrderPackBoxStatusHibernateType>();
                map.Column("status_id_after");
                map.NotNullable(true);
            }
        );

        ManyToOne(
            x => x.User,
            map => {
                map.Class(typeof(UserInfo));
                map.Column("user_id");
                map.NotNullable(true);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        Property(
            x => x.Description,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("description");
                map.NotNullable(true);
                map.Length(500);
            }
        );
    }
}