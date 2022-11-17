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
    /// 是否品牌
    /// </summary>
    [DisplayName("是否品牌")]
    [Required(ErrorMessage = "是否品牌不能为空！")]
    public bool HasBrand { get; set; }

    /// <summary>
    /// 品牌名称
    /// </summary>
    [DisplayName("品牌名称")]
    public string? BrandName { get; set; }

    /// <summary>
    /// 含电池或其他限制
    /// </summary>
    [DisplayName("含电池或其他限制")]
    [Required(ErrorMessage = "含电池或其他限制不能为空！")]
    public bool HasBattery { get; set; }

    /// <summary>
    /// 是否含草木竹
    /// </summary>
    [DisplayName("是否含草木竹")]
    [Required(ErrorMessage = "是否含草木竹不能为空！")]
    public bool HasBamboo { get; set; }

    /// <summary>
    /// 是否含实木包装
    /// </summary>
    [DisplayName("是否含实木包装")]
    [Required(ErrorMessage = "是否含实木包装不能为空！")]
    public bool HasWoodPackage { get; set; }

    /// <summary>
    /// 是否禁止类
    /// </summary>
    [DisplayName("是否禁止类")]
    [Required(ErrorMessage = "是否禁止类不能为空！")]
    public bool IsForbidden { get; set; }

    /// <summary>
    /// 是否特货类
    /// </summary>
    [DisplayName("是否特货类")]
    [Required(ErrorMessage = "是否特货类不能为空！")]
    public bool IsSpecial { get; set; }

    /// <summary>
    /// 创建单号
    /// </summary>
    public string? CreateExpressTrackingNo { get; set; }
}