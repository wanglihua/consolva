using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using shunda.Core;
using shunda.Core.Config;
using shunda.Domain.LogisticsOrder;
using shunda.Domain.Pass;

namespace shunda.Domain.Express;

/// <summary>
/// 快递单尺寸重量
/// </summary>
public class ExpressSizeWeightInfo : Entity
{
    /// <summary>
    /// 快递单，可空
    /// </summary>
    [JsonProperty("express")]
    public virtual ExpressInfo? Express { get; set; } = null;

    /// <summary>
    /// 订单，可空
    /// </summary>
    [JsonProperty("order")]
    public virtual LogisticsOrderInfo? Order { get; set; } = null;

    /// <summary>
    /// 订单编号，非空
    /// </summary>
    [Required(ErrorMessage = "订单编号不能为空！")]
    [MaxLength(100, ErrorMessage = "订单编号长度不能大于100！")]
    [JsonProperty("orderCode")]
    public virtual string OrderCode { get; set; } = string.Empty;

    /// <summary>
    /// 快递单号，非空
    /// </summary>
    [Required(ErrorMessage = "快递单号不能为空！")]
    [MaxLength(30, ErrorMessage = "快递单号长度不能大于30！")]
    [JsonProperty("trackingNo")]
    public virtual string TrackingNo { get; set; } = string.Empty;

    /// <summary>
    /// 快递单子单号，非空
    /// </summary>
    [Required(ErrorMessage = "快递单子单号不能为空！")]
    [MaxLength(30, ErrorMessage = "快递单子单号长度不能大于30！")]
    [JsonProperty("trackingNoSub")]
    public virtual string TrackingNoSub { get; set; } = string.Empty;

    /// <summary>
    /// 快递单描述，非空
    /// </summary>
    [Required(ErrorMessage = "快递单描述不能为空！")]
    [MaxLength(200, ErrorMessage = "快递单描述长度不能大于200！")]
    [JsonProperty("description")]
    public virtual string Description { get; set; } = string.Empty;

    /// <summary>
    /// 长度cm，非空
    /// </summary>
    [Required(ErrorMessage = "长度cm不能为空！")]
    [JsonProperty("length")]
    public virtual decimal Length { get; set; } = 0m;

    /// <summary>
    /// 宽度cm，非空
    /// </summary>
    [Required(ErrorMessage = "宽度cm不能为空！")]
    [JsonProperty("width")]
    public virtual decimal Width { get; set; } = 0m;

    /// <summary>
    /// 高度cm，非空
    /// </summary>
    [Required(ErrorMessage = "高度cm不能为空！")]
    [JsonProperty("height")]
    public virtual decimal Height { get; set; } = 0m;

    /// <summary>
    /// 重量kg，非空
    /// 这个是单件的一个包裹重量，不是多个包裹的总重量
    /// </summary>
    [Required(ErrorMessage = "重量kg不能为空！")]
    [JsonProperty("weight")]
    public virtual decimal Weight { get; set; } = 0m;

    /// <summary>
    /// 入库包裹数，非空
    /// </summary>
    [Required(ErrorMessage = "入库包裹数不能为空！")]
    [JsonProperty("packageCount")]
    public virtual int PackageCount { get; set; } = 0;

    /// <summary>
    /// 添加用户，可空
    /// </summary>
    [JsonProperty("user")]
    public virtual UserInfo? User { get; set; } = null;

    /// <summary>
    /// 自动秤，可空
    /// </summary>
    [JsonProperty("machine")]
    public virtual AutoMeasureMachineInfo? Machine { get; set; } = null;

    #region cache

    /// <summary>
    /// 单箱体积
    /// </summary>
    public virtual decimal GetVolumeCache()
    {
        return decimal.Round(Length * Width * Height / 100_00_00, 6, MidpointRounding.AwayFromZero);
    }

    /// <summary>
    /// 单箱体积重
    /// </summary>
    public virtual decimal GetVolumeWeightCache()
    {
        return decimal.Round(Length * Width * Height / LogisticsConfig.VolumeWeightDenominator, 3, MidpointRounding.AwayFromZero);
    }

    /// <summary>
    /// 体积重实际重比值
    /// </summary>
    public virtual decimal? GetVolumeWeightActualWeightRatioCache()
    {
        if (Weight == 0)
        {
            return null;
        }

        return GetVolumeWeightCache() / Weight;
    }

    /// <summary>
    /// 子单号尺寸重量列表
    /// </summary>
    public virtual IList<ExpressSizeWeightInfo> ExpressSizeWeightChildListCache { get; set; } = new List<ExpressSizeWeightInfo>();

    #endregion
}