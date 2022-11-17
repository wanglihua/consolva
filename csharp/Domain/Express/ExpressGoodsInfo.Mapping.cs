using shunda.Core;
using NHibernate;
using NHibernate.Mapping.ByCode;
using shunda.Domain.Logistics;

namespace shunda.Domain.Express;

public class ExpressGoodsInfoMapping : EntityMapping<ExpressGoodsInfo>
{
    public ExpressGoodsInfoMapping()
    {
        Table("tbl_express_goods");

        ManyToOne(
            x => x.Express,
            map => {
                map.Class(typeof(ExpressInfo));
                map.Column("express_id");
                map.NotNullable(true);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
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

        Property(
            x => x.Price,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("price");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Quantity,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("quantity");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Unit,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("unit");
                map.NotNullable(true);
            }
        );

    }
}