using NHibernate;
using NHibernate.Mapping.ByCode;
using shunda.Core;
using shunda.Domain.Pass;

namespace shunda.Domain.LogisticsOrder;

public class LogisticsOrderRemarkInfoMapping : EntityMapping<LogisticsOrderRemarkInfo> {
    public LogisticsOrderRemarkInfoMapping() {
        Table("tbl_logistics_order_remark");

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

        Property(
            x => x.Type,
            map => {
                map.Type<LogisticsOrderRemarkTypeHibernateType>();
                map.Column("type_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Remark,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("remark");
                map.NotNullable(true);
            }
        );

        ManyToOne(
            x => x.AddUser,
            map => {
                map.Class(typeof(UserInfo));
                map.Column("add_user_id");
                map.NotNullable(false);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );
    }
}