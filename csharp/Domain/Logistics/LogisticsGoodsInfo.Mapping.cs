using shunda.Core;
using NHibernate;
using NHibernate.Mapping.ByCode;
using shunda.Domain.Pass;

namespace shunda.Domain.Logistics;

public class LogisticsGoodsInfoMapping : EntityMapping<LogisticsGoodsInfo>
{
    public LogisticsGoodsInfoMapping()
    {
        Table("tbl_logistics_goods");

        Property(
            x => x.Name,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("name");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Cate,
            map => {
                map.Type<LogisticsGoodsCateHibernateType>();
                map.Column("cate_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Type,
            map => {
                map.Type<LogisticsGoodsTypeHibernateType>();
                map.Column("type_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.BrandName,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("brand_name");
                map.NotNullable(true);
            }
        );

        ManyToOne(
            x => x.HsGoods,
            map => {
                map.Class(typeof(HsGoodsInfo));
                map.Column("hs_goods_id");
                map.NotNullable(false);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        ManyToOne(
            x => x.AddUser,
            map => {
                map.Class(typeof(UserInfo));
                map.Column("add_user_id");
                map.NotNullable(true);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        ManyToOne(
            x => x.UpdateUser,
            map => {
                map.Class(typeof(UserInfo));
                map.Column("update_user_id");
                map.NotNullable(true);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        Property(
            x => x.CreateExpressTrackingNo,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("create_express_tracking_no");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Status,
            map => {
                map.Type<LogisticsGoodsStatusHibernateType>();
                map.Column("status_id");
                map.NotNullable(true);
            }
        );

    }
}