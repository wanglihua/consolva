using System.ComponentModel.DataAnnotations;
using shunda.Core;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using shunda.Domain.Pass;

namespace shunda.Domain.Logistics;

/// <summary>
/// 海关品名
/// </summary>
public class HsGoodsInfo : Entity
{
    /// <summary>
    /// 物品类型，非空
    /// 当前这个字段基本没用起来置空None
    /// </summary>
    [Required(ErrorMessage = "物品类型不能为空！")]
    [JsonProperty("goodsType")]
    [ModelBinder(typeof(LogisticsGoodsTypeModelBinder))]
    public virtual LogisticsGoodsType GoodsType { get; set; } = LogisticsGoodsType.None;

    /// <summary>
    /// 中国海关编码，非空
    /// </summary>
    [Required(ErrorMessage = "中国海关编码不能为空！")]
    [MaxLength(100, ErrorMessage = "中国海关编码长度不能大于100！")]
    [JsonProperty("cnHsCode")]
    public virtual string CnHsCode { get; set; } = string.Empty;

    /// <summary>
    /// 中国海关申报名，非空
    /// </summary>
    [Required(ErrorMessage = "中国海关申报名不能为空！")]
    [MaxLength(200, ErrorMessage = "中国海关申报名长度不能大于200！")]
    [JsonProperty("cnHsName")]
    public virtual string CnHsName { get; set; } = string.Empty;

    /// <summary>
    /// 英文海关编码，非空
    /// </summary>
    [Required(ErrorMessage = "英文海关编码不能为空！")]
    [MaxLength(100, ErrorMessage = "英文海关编码长度不能大于100！")]
    [JsonProperty("enHsCode")]
    public virtual string EnHsCode { get; set; } = string.Empty;

    /// <summary>
    /// 英文海关申报名，非空
    /// </summary>
    [Required(ErrorMessage = "英文海关申报名不能为空！")]
    [MaxLength(200, ErrorMessage = "英文海关申报名长度不能大于200！")]
    [JsonProperty("enHsName")]
    public virtual string EnHsName { get; set; } = string.Empty;

    /// <summary>
    /// 要素1，非空
    /// </summary>
    [Required(ErrorMessage = "要素1不能为空！")]
    [MaxLength(300, ErrorMessage = "要素1长度不能大于300！")]
    [JsonProperty("elem01")]
    public virtual string Elem01 { get; set; } = string.Empty;

    /// <summary>
    /// 要素2，非空
    /// </summary>
    [Required(ErrorMessage = "要素2不能为空！")]
    [MaxLength(300, ErrorMessage = "要素2长度不能大于300！")]
    [JsonProperty("elem02")]
    public virtual string Elem02 { get; set; } = string.Empty;

    /// <summary>
    /// 要素3，非空
    /// </summary>
    [Required(ErrorMessage = "要素3不能为空！")]
    [MaxLength(300, ErrorMessage = "要素3长度不能大于300！")]
    [JsonProperty("elem03")]
    public virtual string Elem03 { get; set; } = string.Empty;

    /// <summary>
    /// 要素4，非空
    /// </summary>
    [Required(ErrorMessage = "要素4不能为空！")]
    [MaxLength(300, ErrorMessage = "要素4长度不能大于300！")]
    [JsonProperty("elem04")]
    public virtual string Elem04 { get; set; } = string.Empty;

    /// <summary>
    /// 要素5，非空
    /// </summary>
    [Required(ErrorMessage = "要素5不能为空！")]
    [MaxLength(300, ErrorMessage = "要素5长度不能大于300！")]
    [JsonProperty("elem05")]
    public virtual string Elem05 { get; set; } = string.Empty;

    /// <summary>
    /// 要素6，非空
    /// </summary>
    [Required(ErrorMessage = "要素6不能为空！")]
    [MaxLength(300, ErrorMessage = "要素6长度不能大于300！")]
    [JsonProperty("elem06")]
    public virtual string Elem06 { get; set; } = string.Empty;

    /// <summary>
    /// 要素7，非空
    /// </summary>
    [Required(ErrorMessage = "要素7不能为空！")]
    [MaxLength(300, ErrorMessage = "要素7长度不能大于300！")]
    [JsonProperty("elem07")]
    public virtual string Elem07 { get; set; } = string.Empty;

    /// <summary>
    /// 要素8，非空
    /// </summary>
    [Required(ErrorMessage = "要素8不能为空！")]
    [MaxLength(300, ErrorMessage = "要素8长度不能大于300！")]
    [JsonProperty("elem08")]
    public virtual string Elem08 { get; set; } = string.Empty;

    /// <summary>
    /// 添加人员，非空
    /// </summary>
    [Required(ErrorMessage = "添加人员不能为空！")]
    [JsonProperty("addUser")]
    public virtual UserInfo AddUser { get; set; } = new ();

}