using NHibernate;
using shunda.Core;

namespace shunda.Domain.Logistics;

public class LogisticsFeeRateInfoMapping : EntityMapping<LogisticsFeeRateInfo>
{
    public LogisticsFeeRateInfoMapping()
    {
        Table("tbl_logistics_fee_rate");

        Property(
            x => x.Name,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("name");
                map.Length(200);
                map.NotNullable(true);
            }
        );

        Property(
            x => x.CareVolumeWeight,
            map => {
                map.Type(NHibernateUtil.Boolean);
                map.Column("care_volume_weight");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.VolumeWeightDenominator,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("volume_weight_denominator");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.CareSideSum,
            map => {
                map.Type(NHibernateUtil.Boolean);
                map.Column("care_side_sum");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.VolumeWeightResultType,
            map => {
                map.Type<VolumeWeightResultTypeHibernateType>();
                map.Column("volume_weight_result_type_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.CustomerLengthDiscount,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("customer_length_discount");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.CustomerWidthDiscount,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("customer_width_discount");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.CustomerHeightDiscount,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("customer_height_discount");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.CustomerWeightDiscount,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("customer_weight_discount");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.ExportLengthDiscount,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("export_length_discount");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.ExportWidthDiscount,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("export_width_discount");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.ExportHeightDiscount,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("export_height_discount");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.ExportWeightDiscount,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("export_weight_discount");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.HasLengthStart,
            map => {
                map.Type(NHibernateUtil.Boolean);
                map.Column("has_length_start");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.LengthStart,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("length_start");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.HasLengthLimit,
            map => {
                map.Type(NHibernateUtil.Boolean);
                map.Column("has_length_limit");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.LengthLimit,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("length_limit");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.CanOverLength,
            map => {
                map.Type(NHibernateUtil.Boolean);
                map.Column("can_over_length");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.OverLengthStep,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("over_length_step");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.OverLengthStepFee,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("over_length_step_fee");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.OverLengthFeeEveryPack,
            map => {
                map.Type(NHibernateUtil.Boolean);
                map.Column("over_length_fee_every_pack");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.HasOverLengthCapAmount,
            map => {
                map.Type(NHibernateUtil.Boolean);
                map.Column("has_over_length_cap_amount");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.OverLengthCapAmount,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("over_length_cap_amount");
                map.NotNullable(false);
            }
        );

        #region 单箱重量限制

        Property(
            x => x.HasSingleWeightStart,
            map => {
                map.Type(NHibernateUtil.Boolean);
                map.Column("has_single_weight_start");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.SingleWeightStart,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("single_weight_start");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.HasSingleWeightLimit,
            map => {
                map.Type(NHibernateUtil.Boolean);
                map.Column("has_single_weight_limit");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.SingleWeightLimit,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("single_weight_limit");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.CanSingleOverWeight,
            map => {
                map.Type(NHibernateUtil.Boolean);
                map.Column("can_single_over_weight");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.SingleOverWeightStep,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("single_over_weight_step");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.SingleOverWeightStepFee,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("single_over_weight_step_fee");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.HasSingleOverWeightCapAmount,
            map => {
                map.Type(NHibernateUtil.Boolean);
                map.Column("has_single_over_weight_cap_amount");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.SingleOverWeightCapAmount,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("single_over_weight_cap_amount");
                map.NotNullable(false);
            }
        );

        #endregion

        #region 平均单箱重量限制

        Property(
            x => x.HasAverageWeightStart,
            map => {
                map.Type(NHibernateUtil.Boolean);
                map.Column("has_average_weight_start");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.AverageWeightStart,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("average_weight_start");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.HasAverageWeightLimit,
            map => {
                map.Type(NHibernateUtil.Boolean);
                map.Column("has_average_weight_limit");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.AverageWeightLimit,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("average_weight_limit");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.CanAverageOverWeight,
            map => {
                map.Type(NHibernateUtil.Boolean);
                map.Column("can_average_over_weight");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.AverageOverWeightStep,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("average_over_weight_step");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.AverageOverWeightStepFee,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("average_over_weight_step_fee");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.HasAverageOverWeightCapAmount,
            map => {
                map.Type(NHibernateUtil.Boolean);
                map.Column("has_average_over_weight_cap_amount");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.AverageOverWeightCapAmount,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("average_over_weight_cap_amount");
                map.NotNullable(false);
            }
        );

        #endregion

        #region 重量限制

        Property(
            x => x.HasWeightStart,
            map => {
                map.Type(NHibernateUtil.Boolean);
                map.Column("has_weight_start");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.WeightStart,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("weight_start");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.HasWeightLimit,
            map => {
                map.Type(NHibernateUtil.Boolean);
                map.Column("has_weight_limit");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.WeightLimit,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("weight_limit");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.CanOverWeight,
            map => {
                map.Type(NHibernateUtil.Boolean);
                map.Column("can_over_weight");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.OverWeightStep,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("over_weight_step");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.OverWeightStepFee,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("over_weight_step_fee");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.HasOverWeightCapAmount,
            map => {
                map.Type(NHibernateUtil.Boolean);
                map.Column("has_over_weight_cap_amount");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.OverWeightCapAmount,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("over_weight_cap_amount");
                map.NotNullable(false);
            }
        );

        #endregion

        Property(
            x => x.VolumeActualWeightMiddle,
            map => {
                map.Type(NHibernateUtil.Boolean);
                map.Column("volume_actual_weight_middle");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.CareVolumeActualWeightRatio,
            map => {
                map.Type(NHibernateUtil.Boolean);
                map.Column("care_volume_actual_weight_ratio");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.MinVolumeActualWeightRatio,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("min_volume_actual_weight_ratio");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.VolumeWeightCareActualWeightBegin,
            map => {
                map.Type(NHibernateUtil.Boolean);
                map.Column("volume_weight_care_actual_weight_begin");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.VolumeWeightActualWeightBegin,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("volume_weight_actual_weight_begin");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.VolumeWeightCareMaxSideLength,
            map => {
                map.Type(NHibernateUtil.Boolean);
                map.Column("volume_weight_care_max_side_length");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.VolumeWeightMaxSideLengthBegin,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("volume_weight_max_side_length_begin");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.VolumeWeightCareSideSumBegin,
            map => {
                map.Type(NHibernateUtil.Boolean);
                map.Column("volume_weight_care_side_sum_begin");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.VolumeWeightSideSumBegin,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("volume_weight_side_sum_begin");
                map.NotNullable(false);
            }
        );        

        Property(
            x => x.CareGoodsVolumePrice,
            map => {
                map.Type(NHibernateUtil.Boolean);
                map.Column("care_goods_volume_price");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.GoodsVolumePrice,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("goods_volume_price");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.HasFreePackageLimit,
            map => {
                map.Type(NHibernateUtil.Boolean);
                map.Column("has_free_package_limit");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.FreePackageLimit,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("free_package_limit");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.ExtraPackageFee,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("extra_package_fee");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.HasExtraPackageFeeCapAmount,
            map => {
                map.Type(NHibernateUtil.Boolean);
                map.Column("has_extra_package_fee_cap_amount");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.ExtraPackageFeeCapAmount,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("extra_package_fee_cap_amount");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.HasFreeStorageDayLimit,
            map => {
                map.Type(NHibernateUtil.Boolean);
                map.Column("has_free_storage_day_limit");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.FreeStorageDayLimit,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("free_storage_day_limit");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.ExtraStorageDayFee,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("extra_storage_day_fee");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.ExtraStorageDayMinFeePerDay,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("extra_storage_day_min_fee_per_day");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.HasExtraStorageDayFeeCapAmount,
            map => {
                map.Type(NHibernateUtil.Boolean);
                map.Column("has_extra_storage_day_fee_cap_amount");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.ExtraStorageDayFeeCapAmount,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("extra_storage_day_fee_cap_amount");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.PublishRate,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("publish_rate");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Status,
            map => {
                map.Type<LogisticsFeeRateStatusHibernateType>();
                map.Column("status_id");
                map.NotNullable(true);
            }
        );
    }
}