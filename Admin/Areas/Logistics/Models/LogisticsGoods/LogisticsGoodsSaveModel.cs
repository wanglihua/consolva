using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using shunda.Domain.Logistics;

namespace shunda.Admin.Areas.Logistics.Models.LogisticsGoods;

public class LogisticsGoodsSaveModel {
    public int LogisticsGoodsId { get; set; }

    /// <summary>
    /// 扩展名，非空
    /// </summary>
    [Required(ErrorMessage = "扩展名不能为空！")]
    [MaxLength(200, ErrorMessage = "扩展名长度不能大于200！")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 物品类型，非空
    /// </summary>
    [Required(ErrorMessage = "物品类型不能为空！")]
    [JsonProperty("type")]
    [ModelBinder(typeof(LogisticsGoodsTypeModelBinder))]
    public virtual LogisticsGoodsType Type { get; set; } = LogisticsGoodsType.None;

    /// <summary>
    /// 品牌名称，非空
    /// </summary>
    [Required(ErrorMessage = "品牌名称不能为空！")]
    [MaxLength(200, ErrorMessage = "品牌名称长度不能大于200！")]
    [JsonProperty("brandName")]
    public virtual string BrandName { get; set; } = string.Empty;

    /// <summary>
    /// 创建单号，非空
    /// </summary>
    [Required(ErrorMessage = "创建单号不能为空！")]
    [MaxLength(30, ErrorMessage = "创建单号长度不能大于30！")]
    [JsonProperty("createExpressTrackingNo")]
    public virtual string CreateExpressTrackingNo { get; set; } = string.Empty;

    /// <summary>
    /// 状态，非空
    /// </summary>
    [Required(ErrorMessage = "状态不能为空！")]
    [JsonProperty("status")]
    [ModelBinder(typeof(LogisticsGoodsStatusModelBinder))]
    public virtual LogisticsGoodsStatus Status { get; set; } = LogisticsGoodsStatus.None;

    /// <summary>
    /// 新添加照片列表
    /// </summary>
    public IList<LogisticsGoodsImageModel> NewImageList { get; set; } = new List<LogisticsGoodsImageModel>();

    #region 海关物品字段

    /// <summary>
    /// 录入方式是自动录入码
    /// </summary>
    public bool HsGoodsAutoSelect { get; set; } = false;

    public int HsGoodsId { get; set; }

    //[Required(ErrorMessage = "中国海关编码不能为空！")]
    [MaxLength(100, ErrorMessage = "中国海关编码长度不能大于100！")]
    public string? CnHsCode { get; set; } = string.Empty;

    //[Required(ErrorMessage = "中国海关申报名不能为空！")]
    [MaxLength(200, ErrorMessage = "中国海关申报名长度不能大于200！")]
    public string? CnHsName { get; set; } = string.Empty;

    //[Required(ErrorMessage = "英文海关编码不能为空！")]
    [MaxLength(100, ErrorMessage = "英文海关编码长度不能大于100！")]
    public string? EnHsCode { get; set; } = string.Empty;

    //[Required(ErrorMessage = "英文海关申报名不能为空！")]
    [MaxLength(200, ErrorMessage = "英文海关申报名长度不能大于200！")]
    public string? EnHsName { get; set; } = string.Empty;

    [MaxLength(300, ErrorMessage = "要素1长度不能大于300！")]
    public string? Elem01 { get; set; }

    [MaxLength(300, ErrorMessage = "要素2长度不能大于300！")]
    public string? Elem02 { get; set; }

    [MaxLength(300, ErrorMessage = "要素3长度不能大于300！")]
    public string? Elem03 { get; set; }

    [MaxLength(300, ErrorMessage = "要素4长度不能大于300！")]
    public string? Elem04 { get; set; }

    [MaxLength(300, ErrorMessage = "要素5长度不能大于300！")]
    public string? Elem05 { get; set; }

    [MaxLength(300, ErrorMessage = "要素6长度不能大于300！")]
    public string? Elem06 { get; set; }

    [MaxLength(300, ErrorMessage = "要素7长度不能大于300！")]
    public string? Elem07 { get; set; }

    [MaxLength(300, ErrorMessage = "要素8长度不能大于300！")]
    public string? Elem08 { get; set; }

    #endregion
}