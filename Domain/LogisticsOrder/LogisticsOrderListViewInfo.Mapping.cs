using NHibernate;
using shunda.Core;
using shunda.Domain.Fee;
using shunda.Domain.Logistics;

namespace shunda.Domain.LogisticsOrder;

public class LogisticsOrderListViewInfoMapping : EntityMapping<LogisticsOrderListViewInfo> {
    public LogisticsOrderListViewInfoMapping() {
        Table("view_logistics_order_list");

        Mutable(false);

        Property(
            x => x.Code,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("code");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.CustomerId,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("customer_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.CustomerCharacter,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("customer_character");
                map.NotNullable(true);
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
            x => x.OrderStatus,
            map => {
                map.Type<LogisticsOrderStatusHibernateType>();
                map.Column("order_status_id");
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
            x => x.ArrearsAmount,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("arrears_amount");
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
            x => x.Country,
            map => {
                map.Type<CountryHibernateType>();
                map.Column("country_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.WarehouseId,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("warehouse_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.WarehouseName,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("warehouse_name");
                map.NotNullable(true);
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

        Property(
            x => x.FirstModeId,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("first_mode_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.FirstModeCode,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("first_mode_code");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.FirstModeName,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("first_mode_name");
                map.NotNullable(true);
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
            x => x.PayTime,
            map => {
                map.Type(NHibernateUtil.DateTime);
                map.Column("pay_time");
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


        Property(
            x => x.TotalActualWeight,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("total_actual_weight");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.TotalVolumeWeight,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("total_volume_weight");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.MaxSideLength,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("max_side_length");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.TotalPackPackageCount,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("total_pack_package_count");
                map.NotNullable(false);
            }
        );
    }
}