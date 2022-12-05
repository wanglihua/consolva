using System.ComponentModel.DataAnnotations;
using shunda.Core;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using shunda.Domain.Express;

namespace shunda.Domain.Logistics;

/// <summary>
/// 物流物品图片
/// </summary>
public class LogisticsGoodsImageInfo : EntityNoUpdate
{
    /// <summary>
    /// 物流物品，非空
    /// </summary>
    [Required(ErrorMessage = "物流物品不能为空！")]
    [JsonProperty("goods")]
    public virtual LogisticsGoodsInfo Goods { get; set; } = new ();

    /// <summary>
    /// 存储路径，非空
    /// </summary>
    [Required(ErrorMessage = "存储路径不能为空！")]
    [MaxLength(255, ErrorMessage = "存储路径长度不能大于255！")]
    [JsonProperty("storePath")]
    public virtual string StorePath { get; set; } = string.Empty;

    /// <summary>
    /// 存储名称，非空
    /// </summary>
    [Required(ErrorMessage = "存储名称不能为空！")]
    [MaxLength(50, ErrorMessage = "存储名称长度不能大于50！")]
    [JsonProperty("storeName")]
    public virtual string StoreName { get; set; } = string.Empty;

    /// <summary>
    /// 存储位置，非空
    /// </summary>
    [Required(ErrorMessage = "存储位置不能为空！")]
    [JsonProperty("storeLoc")]
    [ModelBinder(typeof(ExpressPhotoStoreLocModelBinder))]
    public virtual ExpressPhotoStoreLoc StoreLoc { get; set; } = ExpressPhotoStoreLoc.None;

}