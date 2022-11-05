using shunda.Core;
using NHibernate;
using NHibernate.Mapping.ByCode;
using shunda.Domain.Order;

namespace shunda.Domain.Express;

public class ExpressPhotoInfoMapping : EntityMapping<ExpressPhotoInfo>
{
    public ExpressPhotoInfoMapping()
    {
        Table("tbl_express_photo");

        Property(
            x => x.CustomerId,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("customer_id");
                map.NotNullable(false);
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
            x => x.ExpressTrackingNo,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("express_tracking_no");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.ExpressComName,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("express_com_name");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.Type,
            map => {
                map.Type<ExpressPhotoTypeHibernateType>();
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

        Property(
            x => x.UserId,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("user_id");
                map.NotNullable(false);
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
                map.Type<ExpressPhotoStatusHibernateType>();
                map.Column("status_id");
                map.NotNullable(true);
            }
        );

    }
}