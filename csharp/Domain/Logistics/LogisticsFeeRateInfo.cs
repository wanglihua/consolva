using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using shunda.Core;

namespace shunda.Domain.Logistics;

/// <summary>
/// 物流费率
/// </summary>
public class LogisticsFeeRateInfo : Entity
{
    /// <summary>
    /// 名称
    /// </summary>
    [DisplayName("名称")]
    [Required(ErrorMessage = "名称不能为空！")]
    [MaxLength(200, ErrorMessage = "名称长度不能大于200！")]
    public virtual string Name { get; set; } = string.Empty;

    /// <summary>
    /// 是否考虑体积重
    /// </summary>
    [DisplayName("是否考虑体积重")]
    [Required(ErrorMessage = "是否考虑体积重不能为空！")]
    public virtual bool CareVolumeWeight { get; set; }

    /// <summary>
    /// 体积重分母
    /// </summary>
    [DisplayName("体积重分母")]
    public virtual int? VolumeDenominator { get; set; } = 5000;

    /// <summary>
    /// 是否考虑三边之和
    /// </summary>
    [DisplayName("是否考虑三边之和")]
    [Required(ErrorMessage = "是否考虑三边之和不能为空！")]
    public virtual bool CareSideSum { get; set; }

    /// <summary>
    /// 体积重计算取向
    /// </summary>
    [DisplayName("体积重计算取向")]
    [Required(ErrorMessage = "体积重计算取向不能为空！")]
    [ModelBinder(typeof(VolumeWeightResultTypeModelBinder))]
    public virtual VolumeWeightResultType VolumeWeightResultType { get; set; } = VolumeWeightResultType.None;

    /// <summary>
    /// 客户计算折扣 长
    /// </summary>
    [DisplayName("客户计算折扣 长")]
    [Required(ErrorMessage = "客户计算折扣 长不能为空！")]
    public virtual decimal CustomerLengthDiscount { get; set; } = 1.0m;

    /// <summary>
    /// 客户计算折扣 宽
    /// </summary>
    [DisplayName("客户计算折扣 宽")]
    [Required(ErrorMessage = "客户计算折扣 宽不能为空！")]
    public virtual decimal CustomerWidthDiscount { get; set; } = 1.0m;

    /// <summary>
    /// 客户计算折扣 高
    /// </summary>
    [DisplayName("客户计算折扣 高")]
    [Required(ErrorMessage = "客户计算折扣 高不能为空！")]
    public virtual decimal CustomerHeightDiscount { get; set; } = 1.0m;

    /// <summary>
    /// 客户计算折扣 重
    /// </summary>
    [DisplayName("客户计算折扣 重")]
    [Required(ErrorMessage = "客户计算折扣 重不能为空！")]
    public virtual decimal CustomerWeightDiscount { get; set; } = 1.0m;

    /// <summary>
    /// 导出计算折扣 长
    /// </summary>
    [DisplayName("导出计算折扣 长")]
    [Required(ErrorMessage = "导出计算折扣 长不能为空！")]
    public virtual decimal ExportLengthDiscount { get; set; } = 1.0m;

    /// <summary>
    /// 导出计算折扣 宽
    /// </summary>
    [DisplayName("导出计算折扣 宽")]
    [Required(ErrorMessage = "导出计算折扣 宽不能为空！")]
    public virtual decimal ExportWidthDiscount { get; set; } = 1.0m;

    /// <summary>
    /// 导出计算折扣 高
    /// </summary>
    [DisplayName("导出计算折扣 高")]
    [Required(ErrorMessage = "导出计算折扣 高不能为空！")]
    public virtual decimal ExportHeightDiscount { get; set; } = 1.0m;

    /// <summary>
    /// 导出计算折扣 重
    /// </summary>
    [DisplayName("导出计算折扣 重")]
    [Required(ErrorMessage = "导出计算折扣 重不能为空！")]
    public virtual decimal ExportWeightDiscount { get; set; } = 1.0m;

    /// <summary>
    /// 是否有起步长度
    /// </summary>
    public virtual bool HasLengthStart { get; set; } = false;

    /// <summary>
    /// 长度起步
    /// </summary>
    public virtual int? LengthStart { get; set; }

    /// <summary>
    /// 是否有长度限制
    /// </summary>
    [DisplayName("是否有长度限制")]
    [Required(ErrorMessage = "是否有长度限制不能为空！")]
    public virtual bool HasLengthLimit { get; set; }

    /// <summary>
    /// 长度限制
    /// </summary>
    [DisplayName("长度限制")]
    public virtual int? LengthLimit { get; set; }

    /// <summary>
    /// 能否超长
    /// </summary>
    [DisplayName("能否超长")]
    [Required(ErrorMessage = "能否超长不能为空！")]
    public virtual bool CanOverLength { get; set; }

    /// <summary>
    /// 超长阶梯
    /// </summary>
    [DisplayName("超长阶梯")]
    public virtual decimal? OverLengthStep { get; set; }

    /// <summary>
    /// 超长阶梯收费
    /// </summary>
    [DisplayName("超长阶梯收费")]
    public virtual decimal? OverLengthStepFee { get; set; }

    /// <summary>
    /// 超长是否按件计费
    /// </summary>
    public virtual bool OverLengthFeeEveryPack { get; set; }

    /// <summary>
    /// 是否有超长封顶金额
    /// </summary>
    [DisplayName("是否有超长封顶金额")]
    [Required(ErrorMessage = "是否有超长封顶金额不能为空！")]
    public virtual bool HasOverLengthCapAmount { get; set; }

    /// <summary>
    /// 超长封顶金额
    /// </summary>
    [DisplayName("超长封顶金额")]
    public virtual decimal? OverLengthCapAmount { get; set; }

    #region 单箱重量限制

    /// <summary>
    /// 是否有单箱起步重量
    /// </summary>
    public virtual bool HasSingleWeightStart { get; set; } = false;

    /// <summary>
    /// 单箱起步重量
    /// </summary>
    public virtual int? SingleWeightStart { get; set; }

    /// <summary>
    /// 是否有单箱重量限制
    /// </summary>
    [DisplayName("是否有单箱重量限制")]
    [Required(ErrorMessage = "是否有单箱重量限制不能为空！")]
    public virtual bool HasSingleWeightLimit { get; set; }

    /// <summary>
    /// 单箱重量限制
    /// </summary>
    [DisplayName("单箱重量限制")]
    public virtual int? SingleWeightLimit { get; set; }

    /// <summary>
    /// 能否单箱超重
    /// </summary>
    [DisplayName("能否单箱超重")]
    [Required(ErrorMessage = "能否单箱超重不能为空！")]
    public virtual bool CanSingleOverWeight { get; set; }

    /// <summary>
    /// 单箱超重阶梯
    /// </summary>
    [DisplayName("单箱超重阶梯")]
    public virtual decimal? SingleOverWeightStep { get; set; }

    /// <summary>
    /// 单箱超重阶梯收费
    /// </summary>
    [DisplayName("单箱超重阶梯收费")]
    public virtual decimal? SingleOverWeightStepFee { get; set; }

    /// <summary>
    /// 是否有单箱超重封顶金额
    /// </summary>
    [DisplayName("是否有单箱超重封顶金额")]
    [Required(ErrorMessage = "是否有单箱超重封顶金额不能为空！")]
    public virtual bool HasSingleOverWeightCapAmount { get; set; }

    /// <summary>
    /// 单箱超重封顶金额
    /// </summary>
    [DisplayName("单箱超重封顶金额")]
    public virtual decimal? SingleOverWeightCapAmount { get; set; }

    #endregion

    #region 平均单箱重量限制

    /// <summary>
    /// 是否有平均单箱起步重量
    /// </summary>
    public virtual bool HasAverageWeightStart { get; set; } = false;

    /// <summary>
    /// 平均单箱起步重量
    /// </summary>
    public virtual int? AverageWeightStart { get; set; }

    /// <summary>
    /// 是否有平均单箱重量限制
    /// </summary>
    [DisplayName("是否有平均单箱重量限制")]
    [Required(ErrorMessage = "是否有平均单箱重量限制不能为空！")]
    public virtual bool HasAverageWeightLimit { get; set; }

    /// <summary>
    /// 平均单箱重量限制
    /// </summary>
    [DisplayName("平均单箱重量限制")]
    public virtual int? AverageWeightLimit { get; set; }

    /// <summary>
    /// 能否平均单箱超重
    /// </summary>
    [DisplayName("能否平均单箱超重")]
    [Required(ErrorMessage = "能否平均单箱超重不能为空！")]
    public virtual bool CanAverageOverWeight { get; set; }

    /// <summary>
    /// 平均单箱超重阶梯
    /// </summary>
    [DisplayName("平均单箱超重阶梯")]
    public virtual decimal? AverageOverWeightStep { get; set; }

    /// <summary>
    /// 平均单箱超重阶梯收费
    /// </summary>
    [DisplayName("平均单箱超重阶梯收费")]
    public virtual decimal? AverageOverWeightStepFee { get; set; }

    /// <summary>
    /// 是否有平均单箱超重封顶金额
    /// </summary>
    [DisplayName("是否有平均单箱超重封顶金额")]
    [Required(ErrorMessage = "是否有平均单箱超重封顶金额不能为空！")]
    public virtual bool HasAverageOverWeightCapAmount { get; set; }

    /// <summary>
    /// 平均单箱超重封顶金额
    /// </summary>
    [DisplayName("平均单箱超重封顶金额")]
    public virtual decimal? AverageOverWeightCapAmount { get; set; }

    #endregion

    #region 重量限制

    /// <summary>
    /// 是否有起步重量
    /// </summary>
    public virtual bool HasWeightStart { get; set; } = false;

    /// <summary>
    /// 起步重量
    /// </summary>
    public virtual int? WeightStart { get; set; }

    /// <summary>
    /// 是否有重量限制
    /// </summary>
    [DisplayName("是否有重量限制")]
    [Required(ErrorMessage = "是否有重量限制不能为空！")]
    public virtual bool HasWeightLimit { get; set; }

    /// <summary>
    /// 重量限制
    /// </summary>
    [DisplayName("重量限制")]
    public virtual int? WeightLimit { get; set; }

    /// <summary>
    /// 能否超重
    /// </summary>
    [DisplayName("能否超重")]
    [Required(ErrorMessage = "能否超重不能为空！")]
    public virtual bool CanOverWeight { get; set; }

    /// <summary>
    /// 超重阶梯
    /// </summary>
    [DisplayName("超重阶梯")]
    public virtual decimal? OverWeightStep { get; set; }

    /// <summary>
    /// 超重阶梯收费
    /// </summary>
    [DisplayName("超重阶梯收费")]
    public virtual decimal? OverWeightStepFee { get; set; }

    /// <summary>
    /// 是否有超重封顶金额
    /// </summary>
    [DisplayName("是否有超重封顶金额")]
    [Required(ErrorMessage = "是否有超重封顶金额不能为空！")]
    public virtual bool HasOverWeightCapAmount { get; set; }

    /// <summary>
    /// 超重封顶金额
    /// </summary>
    [DisplayName("超重封顶金额")]
    public virtual decimal? OverWeightCapAmount { get; set; }

    #endregion

    /// <summary>
    /// 体积重实际重折中
    /// </summary>
    public virtual bool VolumeActualWeightMiddle { get; set; }

    /// <summary>
    /// 是否考虑体积重实际重倍率
    /// </summary>
    public virtual bool CareVolumeActualWeightRatio { get; set; }

    /// <summary>
    /// 体积重实际重最小倍率
    /// </summary>
    public virtual decimal? MinVolumeActualWeightRatio { get; set; }

    /// <summary>
    /// 体积重考虑最长边
    /// </summary>
    public virtual bool VolumeWeightCareMaxSideLength { get; set; }

    /// <summary>
    /// 体积重最长边起始
    /// </summary>
    public virtual decimal? VolumeWeightMaxSideLengthBegin { get; set; }

    /// <summary>
    /// 是否启用实际货值与体积货值最大值计算
    /// </summary>
    public virtual bool CareGoodsVolumePrice { get; set; }

    /// <summary>
    /// 体积货值设定
    /// </summary>
    public virtual decimal? GoodsVolumePrice { get; set; }

    /// <summary>
    /// 是否有免费包裹数量限制
    /// </summary>
    [DisplayName("是否有免费包裹数量限制")]
    [Required(ErrorMessage = "是否有免费包裹数量限制不能为空！")]
    public virtual bool HasFreePackageLimit { get; set; }

    /// <summary>
    /// 免费包裹数量限制
    /// </summary>
    [DisplayName("免费包裹数量限制")]
    public virtual int? FreePackageLimit { get; set; }

    /// <summary>
    /// 额外包裹收费
    /// </summary>
    [DisplayName("额外包裹收费")]
    public virtual decimal? ExtraPackageFee { get; set; }

    /// <summary>
    /// 是否有额外包裹收费封顶金额
    /// </summary>
    [DisplayName("是否有额外包裹收费封顶金额")]
    [Required(ErrorMessage = "是否有额外包裹收费封顶金额不能为空！")]
    public virtual bool HasExtraPackageFeeCapAmount { get; set; }

    /// <summary>
    /// 额外包裹收费封顶金额
    /// </summary>
    [DisplayName("额外包裹收费封顶金额")]
    public virtual decimal? ExtraPackageFeeCapAmount { get; set; }

    /// <summary>
    /// 是否有免费仓储天数限制
    /// </summary>
    [DisplayName("是否有免费仓储天数限制")]
    [Required(ErrorMessage = "是否有免费仓储天数限制不能为空！")]
    public virtual bool HasFreeStorageDayLimit { get; set; }

    /// <summary>
    /// 免费仓储天数限制
    /// </summary>
    [DisplayName("免费仓储天数限制")]
    public virtual int? FreeStorageDayLimit { get; set; }

    /// <summary>
    /// 额外仓储收费
    /// RMB/天/立方
    /// </summary>
    [DisplayName("额外仓储收费")]
    public virtual decimal? ExtraStorageDayFee { get; set; }

    /// <summary>
    /// 最低仓储收费
    /// RMB/天
    /// </summary>
    public virtual decimal? ExtraStorageDayMinFeePerDay { get; set; }

    /// <summary>
    /// 是否有额外仓储收费封顶金额
    /// </summary>
    [DisplayName("是否有额外仓储收费封顶金额")]
    [Required(ErrorMessage = "是否有额外仓储收费封顶金额不能为空！")]
    public virtual bool HasExtraStorageDayFeeCapAmount { get; set; }

    /// <summary>
    /// 额外仓储收费封顶金额
    /// </summary>
    [DisplayName("额外仓储收费封顶金额")]
    public virtual decimal? ExtraStorageDayFeeCapAmount { get; set; }

    /// <summary>
    /// 普通类是否允许
    /// </summary>
    [DisplayName("普通类是否允许")]
    [Required(ErrorMessage = "普通类是否允许不能为空！")]
    public virtual bool CommonGoodsPermit { get; set; }

    /// <summary>
    /// 特货类是否允许
    /// </summary>
    [DisplayName("特货类是否允许")]
    [Required(ErrorMessage = "特货类是否允许不能为空！")]
    public virtual bool SpecialGoodsPermit { get; set; }

    /// <summary>
    /// 特货类是否开启按件加收费用
    /// </summary>
    [DisplayName("特货类是否开启按件加收费用")]
    [Required(ErrorMessage = "特货类是否开启按件加收费用不能为空！")]
    public virtual bool SpecialGoodsByCount { get; set; }

    /// <summary>
    /// 特货类按件加收费用
    /// </summary>
    [DisplayName("特货类按件加收费用")]
    public virtual decimal? SpecialGoodsByCountFee { get; set; }

    /// <summary>
    /// 特货类是否开启按体积加收费用
    /// </summary>
    [DisplayName("特货类是否开启按体积加收费用")]
    [Required(ErrorMessage = "特货类是否开启按体积加收费用不能为空！")]
    public virtual bool SpecialGoodsByVolume { get; set; }

    /// <summary>
    /// 特货类按体积加收费用
    /// </summary>
    [DisplayName("特货类按体积加收费用")]
    public virtual decimal? SpecialGoodsByVolumeFee { get; set; }

    /// <summary>
    /// 特货类是否开启按金额加收费用
    /// </summary>
    [DisplayName("特货类是否开启按金额加收费用")]
    [Required(ErrorMessage = "特货类是否开启按金额加收费用不能为空！")]
    public virtual bool SpecialGoodsByAmount { get; set; }

    /// <summary>
    /// 特货类按金额加收费用百分比
    /// </summary>
    [DisplayName("特货类按金额加收费用百分比")]
    public virtual decimal? SpecialGoodsByAmountFeePercent { get; set; }

    /// <summary>
    /// 是否有特货类加收费用封顶金额
    /// </summary>
    [DisplayName("是否有特货类加收费用封顶金额")]
    [Required(ErrorMessage = "是否有特货类加收费用封顶金额不能为空！")]
    public virtual bool HasSpecialGoodsCapAmount { get; set; }

    /// <summary>
    /// 特货类加收费用封顶金额
    /// </summary>
    [DisplayName("特货类加收费用封顶金额")]
    public virtual decimal? SpecialGoodsCapAmount { get; set; }

    /// <summary>
    /// 公布系数
    /// </summary>
    [DisplayName("公布系数")]
    public virtual decimal PublishRate { get; set; } = 1.0m;

    /// <summary>
    /// 状态
    /// </summary>
    [DisplayName("状态")]
    [ModelBinder(typeof(LogisticsFeeRateStatusModelBinder))]
    public virtual LogisticsFeeRateStatus Status { get; set; } = LogisticsFeeRateStatus.None;
}