using shunda.Core;
using NHibernate;
using NHibernate.Mapping.ByCode;

namespace shunda.Domain.Express;

public class ExpressRemarkInfoMapping : EntityMapping<ExpressRemarkInfo>
{
    public ExpressRemarkInfoMapping()
    {
        Table("tbl_express_remark");

        ManyToOne(
            x => x.Express,
            map => {
                map.Class(typeof(ExpressInfo));
                map.Column("express_id");
                map.NotNullable(false);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        ManyToOne(
            x => x.Order,
            map => {
                map.Class(typeof(LogisticsOrderInfo));
                map.Column("order_id");
                map.NotNullable(false);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        Property(
            x => x.Type,
            map => {
                map.Type<ExpressRemarkTypeHibernateType>();
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