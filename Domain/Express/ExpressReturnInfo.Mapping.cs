using shunda.Core;
using NHibernate;
using NHibernate.Mapping.ByCode;
using shunda.Domain.Customer;
using shunda.Domain.Logistics;
using shunda.Domain.Pass;
using shunda.Domain.Storage;

namespace shunda.Domain.Express;

public class ExpressReturnInfoMapping : EntityMapping<ExpressReturnInfo> {
    public ExpressReturnInfoMapping() {
        Table("tbl_express_return");

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

        Property(
            x => x.TrackingNo,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("tracking_no");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.CheckinTrackingNo,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("checkin_tracking_no");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.ExpressCompany,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("express_company");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.GoodsType,
            map => {
                map.Type<LogisticsGoodsTypeHibernateType>();
                map.Column("goods_type_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.AddTime,
            map => {
                map.Type(NHibernateUtil.DateTime);
                map.Column("add_time");
                map.NotNullable(true);
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
            x => x.Warehouse,
            map => {
                map.Class(typeof(WarehouseInfo));
                map.Column("warehouse_id");
                map.NotNullable(true);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

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

        Property(
            x => x.Status,
            map => {
                map.Type<ExpressStatusHibernateType>();
                map.Column("status_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.ReturnTrackingNo,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("return_tracking_no");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.ReturnExpressCompany,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("return_express_company");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.ReturnApplyTime,
            map => {
                map.Type(NHibernateUtil.DateTime);
                map.Column("return_apply_time");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.ReturnPickupCode,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("return_pickup_code");
                map.NotNullable(false);
            }
        );

        ManyToOne(
            x => x.ReturnUser,
            map => {
                map.Class(typeof(CustomerInfo));
                map.Column("return_user_id");
                map.NotNullable(false);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        Property(
            x => x.ReturnTime,
            map => {
                map.Type(NHibernateUtil.DateTime);
                map.Column("return_time");
                map.NotNullable(false);
            }
        );

    }
}