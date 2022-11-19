using shunda.Core;
using NHibernate;
using NHibernate.Mapping.ByCode;
using shunda.Domain.Customer;
using shunda.Domain.Logistics;
using shunda.Domain.LogisticsOrder;
using shunda.Domain.Pass;
using shunda.Domain.Storage;

namespace shunda.Domain.Express;

public class ExpressInfoMapping : EntityMapping<ExpressInfo>
{
    public ExpressInfoMapping()
    {
        Table("tbl_express");

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
            x => x.Description,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("description");
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

        Property(
            x => x.BillCustomerCode,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("bill_customer_code");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.BillCustomerId,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("bill_customer_id");
                map.NotNullable(true);
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
            x => x.Shelf,
            map => {
                map.Class(typeof(ShelfInfo));
                map.Column("shelf_id");
                map.NotNullable(true);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        ManyToOne(
            x => x.StorageLoc,
            map => {
                map.Class(typeof(StorageLocInfo));
                map.Column("storage_loc_id");
                map.NotNullable(true);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        Property(
            x => x.PackageCount,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("package_count");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.ClaimTime,
            map => {
                map.Type(NHibernateUtil.DateTime);
                map.Column("claim_time");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.CustomerIgnored,
            map => {
                map.Type(NHibernateUtil.Boolean);
                map.Column("customer_ignored");
                map.NotNullable(true);
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
            x => x.FirstType,
            map => {
                map.Type<LogisticsModeTypeHibernateType>();
                map.Column("first_type_id");
                map.NotNullable(true);
            }
        );

        ManyToOne(
            x => x.FirstMode,
            map => {
                map.Class(typeof(LogisticsModeInfo));
                map.Column("first_mode_id");
                map.NotNullable(false);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        Property(
            x => x.SubmitPackTime,
            map => {
                map.Type(NHibernateUtil.DateTime);
                map.Column("submit_pack_time");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.TotalWeight,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("total_weight");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.TotalVolume,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("total_volume");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.OnShelfTime,
            map => {
                map.Type(NHibernateUtil.DateTime);
                map.Column("on_shelf_time");
                map.NotNullable(false);
            }
        );

        ManyToOne(
            x => x.OnShelfUser,
            map => {
                map.Class(typeof(UserInfo));
                map.Column("on_shelf_user_id");
                map.NotNullable(false);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        Property(
            x => x.OffShelfTime,
            map => {
                map.Type(NHibernateUtil.DateTime);
                map.Column("off_shelf_time");
                map.NotNullable(false);
            }
        );

        ManyToOne(
            x => x.OffShelfUser,
            map => {
                map.Class(typeof(UserInfo));
                map.Column("off_shelf_user_id");
                map.NotNullable(false);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        Property(
            x => x.PackingTime,
            map => {
                map.Type(NHibernateUtil.DateTime);
                map.Column("packing_time");
                map.NotNullable(false);
            }
        );

        ManyToOne(
            x => x.PackingUser,
            map => {
                map.Class(typeof(UserInfo));
                map.Column("packing_user_id");
                map.NotNullable(false);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        Property(
            x => x.ShelfStatus,
            map => {
                map.Type<ExpressShelfStatusHibernateType>();
                map.Column("shelf_status_id");
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
                map.NotNullable(false);
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

        Property(
            x => x.ReturnUserId,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("return_user_id");
                map.NotNullable(false);
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

        ManyToOne(
            x => x.DumpUser,
            map => {
                map.Class(typeof(UserInfo));
                map.Column("dump_user_id");
                map.NotNullable(false);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        Property(
            x => x.DumpTime,
            map => {
                map.Type(NHibernateUtil.DateTime);
                map.Column("dump_time");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.DumpStatus,
            map => {
                map.Type<ExpressDumpStatusHibernateType>();
                map.Column("dump_status_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.ArchiveType,
            map => {
                map.Type<ExpressArchiveTypeHibernateType>();
                map.Column("archive_type_id");
                map.NotNullable(true);
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

    }
}