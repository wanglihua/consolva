using NHibernate;
using NHibernate.Mapping.ByCode;
using shunda.Core;
using shunda.Domain.Customer;
using shunda.Domain.Order;
using shunda.Domain.Pass;

namespace shunda.Domain.Express;

public class ExpressVideoInfoMapping : EntityMapping<ExpressVideoInfo>
{
    public ExpressVideoInfoMapping()
    {
        Table("tbl_express_video");

        ManyToOne(
            x => x.Customer,
            map => {
                map.Class(typeof(CustomerInfo));
                map.Column("customer_id");
                map.NotNullable(false);
                map.Fetch(FetchKind.Select);
                map.Cascade(Cascade.None);
            }
        );

        ManyToOne(
            x => x.Order,
            map => {
                map.Class(typeof(LogisticsOrderInfo));
                map.Column("order_id");
                map.NotNullable(false);
                map.Fetch(FetchKind.Select);
                map.Cascade(Cascade.None);
            }
        );

        ManyToOne(
            x => x.Express,
            map => {
                map.Class(typeof(ExpressInfo));
                map.Column("express_id");
                map.NotNullable(false);
                map.Fetch(FetchKind.Select);
                map.Cascade(Cascade.None);
            }
        );

        Property(
            x => x.ExpressTrackingNo,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("express_tracking_no");
                map.Length(30);
                map.NotNullable(false);
            }
        );

        Property(
            x => x.ExpressComName,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("express_com_name");
                map.Length(100);
                map.NotNullable(false);
            }
        );

        Property(
            x => x.Type,
            map => {
                map.Type<ExpressVideoTypeHibernateType>();
                map.Column("type_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.StoreOss,
            map => {
                map.Type(NHibernateUtil.Boolean);
                map.Column("store_oss");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.StorePath,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("store_path");
                map.Length(255);
                map.NotNullable(true);
            }
        );

        Property(
            x => x.StoreName,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("store_name");
                map.Length(50);
                map.NotNullable(true);
            }
        );

        ManyToOne(
            x => x.User,
            map => {
                map.Class(typeof(UserInfo));
                map.Column("user_id");
                map.NotNullable(false);
                map.Fetch(FetchKind.Select);
                map.Cascade(Cascade.None);
            }
        );

        Property(
            x => x.Status,
            map => {
                map.Type<ExpressVideoStatusHibernateType>();
                map.Column("status_id");
                map.NotNullable(true);
            }
        );
    }
}