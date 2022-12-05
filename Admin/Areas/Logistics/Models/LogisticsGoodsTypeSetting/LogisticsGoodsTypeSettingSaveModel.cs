using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using shunda.Domain.Logistics;

namespace shunda.Admin.Areas.Logistics.Models.LogisticsGoodsTypeSetting;

public class LogisticsGoodsTypeSettingSaveModel
{
    /// <summary>
    /// Id
    /// </summary>
    public int LogisticsGoodsTypeSettingId { get; set; }

    /// <summary>
    /// 物品类型，非空
    /// </summary>
    [Required(ErrorMessage = "物品类型不能为空！")]
    [ModelBinder(typeof(LogisticsGoodsTypeModelBinder))]
    public LogisticsGoodsType Type { get; set; } = LogisticsGoodsType.None;

    /// <summary>
    /// 物品类型说明，非空
    /// </summary>
    [Required(ErrorMessage = "物品类型说明不能为空！")]
    [MaxLength(800, ErrorMessage = "物品类型说明长度不能大于800！")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// 常见品名，非空
    /// </summary>
    [Required(ErrorMessage = "常见品名不能为空！")]
    [MaxLength(800, ErrorMessage = "常见品名长度不能大于800！")]
    public string SampleGoods { get; set; } = string.Empty;

    /// <summary>
    /// 状态，非空
    /// </summary>
    [Required(ErrorMessage = "状态不能为空！")]
    [ModelBinder(typeof(LogisticsGoodsTypeStatusModelBinder))]
    public LogisticsGoodsTypeStatus Status { get; set; } = LogisticsGoodsTypeStatus.None;

}