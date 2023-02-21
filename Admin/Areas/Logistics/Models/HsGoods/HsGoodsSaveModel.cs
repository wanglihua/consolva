using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace shunda.Admin.Areas.Logistics.Models.HsGoods;

public class HsGoodsSaveModel {
    /// <summary>
    /// HsGoodsId
    /// </summary>
    public int HsGoodsId { get; set; }

    /// <summary>
    /// 中国海关编码
    /// </summary>
    [Required(ErrorMessage = "中国海关编码不能为空！")]
    [MaxLength(100, ErrorMessage = "中国海关编码长度不能大于100！")]
    [JsonProperty("cnHsCode")]
    public string CnHsCode { get; set; } = string.Empty;

    /// <summary>
    /// 中国海关申报名
    /// </summary>
    [Required(ErrorMessage = "中国海关申报名不能为空！")]
    [MaxLength(200, ErrorMessage = "中国海关申报名长度不能大于200！")]
    [JsonProperty("cnHsName")]
    public string CnHsName { get; set; } = string.Empty;

    /// <summary>
    /// 英文海关编码
    /// </summary>
    [Required(ErrorMessage = "英文海关编码不能为空！")]
    [MaxLength(100, ErrorMessage = "英文海关编码长度不能大于100！")]
    [JsonProperty("enHsCode")]
    public string EnHsCode { get; set; } = string.Empty;

    /// <summary>
    /// 英文海关申报名
    /// </summary>
    [Required(ErrorMessage = "英文海关申报名不能为空！")]
    [MaxLength(200, ErrorMessage = "英文海关申报名长度不能大于200！")]
    [JsonProperty("enHsName")]
    public string EnHsName { get; set; } = string.Empty;

    /// <summary>
    /// 要素1
    /// </summary>
    [MaxLength(300, ErrorMessage = "要素1长度不能大于300！")]
    [JsonProperty("elem01")]
    public string Elem01 { get; set; } = string.Empty;

    /// <summary>
    /// 要素2
    /// </summary>
    [MaxLength(300, ErrorMessage = "要素2长度不能大于300！")]
    [JsonProperty("elem02")]
    public string Elem02 { get; set; } = string.Empty;

    /// <summary>
    /// 要素3
    /// </summary>
    [MaxLength(300, ErrorMessage = "要素3长度不能大于300！")]
    [JsonProperty("elem03")]
    public string Elem03 { get; set; } = string.Empty;

    /// <summary>
    /// 要素4
    /// </summary>
    [MaxLength(300, ErrorMessage = "要素4长度不能大于300！")]
    [JsonProperty("elem04")]
    public string Elem04 { get; set; } = string.Empty;

    /// <summary>
    /// 要素5
    /// </summary>
    [MaxLength(300, ErrorMessage = "要素5长度不能大于300！")]
    [JsonProperty("elem05")]
    public string Elem05 { get; set; } = string.Empty;

    /// <summary>
    /// 要素6
    /// </summary>
    [MaxLength(300, ErrorMessage = "要素6长度不能大于300！")]
    [JsonProperty("elem06")]
    public string Elem06 { get; set; } = string.Empty;

    /// <summary>
    /// 要素7
    /// </summary>
    [MaxLength(300, ErrorMessage = "要素7长度不能大于300！")]
    [JsonProperty("elem07")]
    public string Elem07 { get; set; } = string.Empty;

    /// <summary>
    /// 要素8
    /// </summary>
    [MaxLength(300, ErrorMessage = "要素8长度不能大于300！")]
    [JsonProperty("elem08")]
    public string Elem08 { get; set; } = string.Empty;
}