using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace shunda.Admin.Areas.ReceivePack.Models.ExpressCheckin;

public class LogisticsExpressCheckinSizeWeightModel
{
    /// <summary>
    /// 快递单号
    /// </summary>
    [Required(ErrorMessage = "快递单号不能为空！")]
    public string ExpressTrackingNo { get; set; } = string.Empty;

    /// <summary>
    /// 长度cm，可空
    /// </summary>
    [Required(ErrorMessage = "长度不能为空！")]
    public decimal? Length { get; set; }

    /// <summary>
    /// 宽度cm，可空
    /// </summary>
    [Required(ErrorMessage = "宽度不能为空！")]
    public decimal? Width { get; set; }

    /// <summary>
    /// 高度cm，可空
    /// </summary>
    [Required(ErrorMessage = "高度不能为空！")]
    public decimal? Height { get; set; }

    /// <summary>
    /// 重量kg，可空
    /// </summary>
    [Required(ErrorMessage = "重量不能为空！")]
    public decimal? Weight { get; set; }

    /// <summary>
    /// 箱数，可空
    /// </summary>
    [Required(ErrorMessage = "箱数不能为空！")]
    public int? PackageCount { get; set; }

    /// <summary>
    /// 子单号尺寸重量
    /// </summary>
    public IList<LogisticsExpressCheckinSizeWeightChildItem> ChildSizeWeightList { get; set; } = new List<LogisticsExpressCheckinSizeWeightChildItem>();
}

public class LogisticsExpressCheckinSizeWeightChildItem
{
    [Required(ErrorMessage = "子单号ID不能为空！")]
    public int ExpressChildId { get; set; }

    [BindNever]
    public string ParentTrackingNo { get; set; } = string.Empty;

    /// <summary>
    /// 子单号快递单号
    /// </summary>
    [Required(ErrorMessage = "子单号快递单号不能为空！")]
    public string TrackingNo { get; set; } = string.Empty;

    /// <summary>
    /// 长度cm，可空
    /// </summary>
    [Required(ErrorMessage = "子单号长度不能为空！")]
    public decimal? Length { get; set; }

    /// <summary>
    /// 宽度cm，可空
    /// </summary>
    [Required(ErrorMessage = "子单号宽度不能为空！")]
    public decimal? Width { get; set; }

    /// <summary>
    /// 高度cm，可空
    /// </summary>
    [Required(ErrorMessage = "子单号高度不能为空！")]
    public decimal? Height { get; set; }

    /// <summary>
    /// 重量kg，可空
    /// </summary>
    [Required(ErrorMessage = "子单号重量不能为空！")]
    public decimal? Weight { get; set; }

    /// <summary>
    /// 箱数，可空
    /// </summary>
    [Required(ErrorMessage = "箱数不能为空！")]
    public int? PackageCount { get; set; }
}