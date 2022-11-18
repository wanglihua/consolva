using shunda.Core;
using NHibernate;
using NHibernate.Mapping.ByCode;
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
            x => x.OrderCode,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("order_code");
                map.NotNullable(true);
            }
        );

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

        Property(
            x => x.TrackingNo,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("tracking_no");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.ExpressCompany,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("express_company");
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
            x => x.StorePath,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("store_path");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.StoreName,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("store_name");
                map.NotNullable(true);
            }
        );

        ManyToOne(
            x => x.User,
            map => {
                map.Class(typeof(UserInfo));
                map.Column("user_id");
                map.NotNullable(false);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        Property(
            x => x.StoreLoc,
            map => {
                map.Type<ExpressPhotoStoreLocHibernateType>();
                map.Column("store_loc_id");
                map.NotNullable(true);
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