using shunda.Core;
using NHibernate;
using NHibernate.Mapping.ByCode;
using shunda.Domain.Customer;
using shunda.Domain.Fee;
using shunda.Domain.Logistics;
using shunda.Domain.Storage;

namespace shunda.Domain.LogisticsOrder;

public class LogisticsOrderInfoMapping : EntityMapping<LogisticsOrderInfo> {
    public LogisticsOrderInfoMapping() {
        Table("tbl_logistics_order");

        Property(
            x => x.Code,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("code");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Country,
            map => {
                map.Type<CountryHibernateType>();
                map.Column("country_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.OrderCate,
            map => {
                map.Type<LogisticsOrderCateHibernateType>();
                map.Column("order_cate_id");
                map.NotNullable(true);
            }
        );

        ManyToOne(
            x => x.Customer,
            map => {
                map.Class(typeof(CustomerInfo));
                map.Column("customer_id");
                map.NotNullable(true);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        Property(
            x => x.CustomerCode,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("customer_code");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.FeeMethod,
            map => {
                map.Type<LogisticsOrderFeeMethodHibernateType>();
                map.Column("fee_method_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.CreateMethod,
            map => {
                map.Type<LogisticsOrderCreateMethodHibernateType>();
                map.Column("create_method_id");
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

        Property(
            x => x.FirstModeType,
            map => {
                map.Type<LogisticsModeTypeHibernateType>();
                map.Column("first_mode_type_id");
                map.NotNullable(true);
            }
        );

        ManyToOne(
            x => x.FirstMode,
            map => {
                map.Class(typeof(LogisticsModeInfo));
                map.Column("first_mode_id");
                map.NotNullable(true);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        ManyToOne(
            x => x.Insurance,
            map => {
                map.Class(typeof(LogisticsInsuranceInfo));
                map.Column("insurance_id");
                map.NotNullable(true);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        ManyToOne(
            x => x.Gst,
            map => {
                map.Class(typeof(LogisticsGstInfo));
                map.Column("gst_id");
                map.NotNullable(true);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
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
            x => x.OrderStatus,
            map => {
                map.Type<LogisticsOrderStatusHibernateType>();
                map.Column("order_status_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.PayStatus,
            map => {
                map.Type<PayStatusHibernateType>();
                map.Column("pay_status_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.PayType,
            map => {
                map.Type<PayTypeHibernateType>();
                map.Column("pay_type_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.GoodsValue,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("goods_value");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.OrderAmount,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("order_amount");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.PaidAmount,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("paid_amount");
                map.NotNullable(true);
            }
        );

        // 作废订单欠费金额是零
        Property(
            x => x.ArrearsAmountFormula,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Formula($"if(order_status_id = {LogisticsOrderStatus.Canceled.Id}, 0, if(order_amount - paid_amount > 0, order_amount - paid_amount, 0))");
            }
        );

        Property(
            x => x.RepackType,
            map => {
                map.Type<LogisticsRepackTypeHibernateType>();
                map.Column("repack_type_id");
                map.NotNullable(true);
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
            x => x.StorageLocCount,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("storage_loc_count");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.PickupCode,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("pickup_code");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.PayTime,
            map => {
                map.Type(NHibernateUtil.DateTime);
                map.Column("pay_time");
                map.NotNullable(false);
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
            x => x.CompletePackTime,
            map => {
                map.Type(NHibernateUtil.DateTime);
                map.Column("complete_pack_time");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.TransferTime,
            map => {
                map.Type(NHibernateUtil.DateTime);
                map.Column("transfer_time");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.DeliverTime,
            map => {
                map.Type(NHibernateUtil.DateTime);
                map.Column("deliver_time");
                map.NotNullable(false);
            }
        );

        Bag(
            e => e.PackSchedules,
            mapper => {
                mapper.Key(k => k.Column("order_id"));
                mapper.Inverse(true);
                mapper.Cascade(Cascade.None);
                mapper.Fetch(CollectionFetchMode.Select);
            },
            relation => relation.OneToMany(
                mapping => mapping.Class(typeof(LogisticsOrderPackScheduleInfo))
            )
        );
    }
}