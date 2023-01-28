using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using shunda.Core;
using shunda.Domain.Pass;

namespace shunda.Domain.LogisticsOrder;

/// <summary>
/// 物流订单打包汇总数据
/// </summary>
public class LogisticsOrderPackDataInfo : Entity {
    /// <summary>
    /// 订单，非空
    /// </summary>
    [Required(ErrorMessage = "订单不能为空！")]
    [JsonProperty("order")]
    public virtual LogisticsOrderInfo Order { get; set; } = new LogisticsOrderInfo();

    /// <summary>
    /// 实际重量 kg，非空
    /// </summary>
    [Required(ErrorMessage = "实际重量不能为空！")]
    [JsonProperty("totalActualWeight")]
    public virtual decimal TotalActualWeight { get; set; } = 0m;

    /// <summary>
    /// 体积重 kg，非空
    /// </summary>
    [Required(ErrorMessage = "体积重不能为空！")]
    [JsonProperty("totalVolumeWeight")]
    public virtual decimal TotalVolumeWeight { get; set; } = 0m;

    /// <summary>
    /// 总体积 cubic meter，非空
    /// </summary>
    [Required(ErrorMessage = "总体积不能为空！")]
    [JsonProperty("totalVolume")]
    public virtual decimal TotalVolume { get; set; } = 0m;

    /// <summary>
    /// 最长边 cm，非空
    /// </summary>
    [Required(ErrorMessage = "最长边不能为空！")]
    [JsonProperty("maxSideLength")]
    public virtual decimal MaxSideLength { get; set; } = 0m;

    /// <summary>
    /// 单箱最大体积 cubic meter，非空
    /// </summary>
    [Required(ErrorMessage = "单箱最大体积不能为空！")]
    [JsonProperty("maxVolumeSinglePack")]
    public virtual decimal MaxVolumeSinglePack { get; set; } = 0m;

    /// <summary>
    /// 单箱最大重量 kg，非空
    /// </summary>
    [Required(ErrorMessage = "单箱最大重量不能为空！")]
    [JsonProperty("maxWeightSinglePack")]
    public virtual decimal MaxWeightSinglePack { get; set; } = 0m;

    /// <summary>
    /// 总箱数，非空
    /// </summary>
    [Required(ErrorMessage = "总箱数不能为空！")]
    [JsonProperty("totalPackageCount")]
    public virtual int TotalPackageCount { get; set; } = 0;

    /// <summary>
    /// 打包用户，非空
    /// </summary>
    [Required(ErrorMessage = "打包用户不能为空！")]
    [JsonProperty("packUser")]
    public virtual UserInfo PackUser { get; set; } = new UserInfo();

}