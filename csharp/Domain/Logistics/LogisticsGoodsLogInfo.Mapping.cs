using shunda.Core;
using NHibernate;
using NHibernate.Mapping.ByCode;
using shunda.Domain.Pass;

namespace shunda.Domain.Logistics;

public class LogisticsGoodsLogInfoMapping : EntityNoUpdateMapping<LogisticsGoodsLogInfo>
{
    public LogisticsGoodsLogInfoMapping()
    {
        Table("tbl_logistics_goods_log");

        Property(
            x => x.Type,
            map => {
                map.Type<LogisticsGoodsLogTypeHibernateType>();
                map.Column("type_id");
                map.NotNullable(true);
            }
        );

        ManyToOne(
            x => x.Goods,
            map => {
                map.Class(typeof(LogisticsGoodsInfo));
                map.Column("goods_id");
                map.NotNullable(true);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
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
            }
        );

    }
}