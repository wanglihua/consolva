using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Logistics;

namespace shunda.Admin.Areas.Logistics.Models.LogisticsGoodsWarehouse;

public class LogisticsGoodsWarehouseDetailModel
{
    [BindNever]
    public IList<LogisticsGoodsType> TypeList { get; set; } = LogisticsGoodsType.GetInnerList();

    [BindNever]
    public IList<LogisticsGoodsImageInfo> ImageList { get; set; } = new List<LogisticsGoodsImageInfo>();

    public IList<LogisticsGoodsWarehouseImageModel> NewImageList { get; set; } = new List<LogisticsGoodsWarehouseImageModel>();

    /// <summary>
    /// 物品 id
    /// </summary>
    public int GoodsId { get; set; }

    /// <summary>
    /// 扩展名
    /// </summary>
    [DisplayName("扩展名")]
    [Required(ErrorMessage = "扩展名不能为空！")]
    [MaxLength(200, ErrorMessage = "扩展名长度不能大于200！")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 物品类型
    /// </summary>
    [DisplayName("物品类型")]
    [Required(ErrorMessage = "物品类型不能为空！")]
    [ModelBinder(typeof(LogisticsGoodsTypeModelBinder))]
    public LogisticsGoodsType Type { get; set; } = LogisticsGoodsType.None;

    /// <summary>
    /// 品牌名称
    /// </summary>
    [DisplayName("品牌名称")]
    public string? BrandName { get; set; }

    /// <summary>
    /// 创建单号
    /// </summary>
    public string? CreateExpressTrackingNo { get; set; }
}