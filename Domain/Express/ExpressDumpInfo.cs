using System.ComponentModel.DataAnnotations;
using shunda.Core;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using shunda.Domain.Pass;
using shunda.Domain.Storage;

namespace shunda.Domain.Express;

/// <summary>
/// 快递堆存
/// </summary>
public class ExpressDumpInfo : Entity
{
    /// <summary>
    /// 类型，非空
    /// </summary>
    [Required(ErrorMessage = "类型不能为空！")]
    [JsonProperty("type")]
    [ModelBinder(typeof(ExpressDumpTypeModelBinder))]
    public virtual ExpressDumpType Type { get; set; } = ExpressDumpType.None;

    /// <summary>
    /// 申领件，非空
    /// </summary>
    [Required(ErrorMessage = "申领件不能为空！")]
    [JsonProperty("express")]
    public virtual ExpressInfo Express { get; set; } = new ();

    /// <summary>
    /// 快递单号，非空
    /// </summary>
    [Required(ErrorMessage = "快递单号不能为空！")]
    [MaxLength(30, ErrorMessage = "快递单号长度不能大于30！")]
    [JsonProperty("trackingNo")]
    public virtual string TrackingNo { get; set; } = string.Empty;

    /// <summary>
    /// 堆存用户，非空
    /// </summary>
    [Required(ErrorMessage = "堆存用户不能为空！")]
    [JsonProperty("dumpUser")]
    public virtual UserInfo DumpUser { get; set; } = new ();

    /// <summary>
    /// 堆存时间，非空
    /// </summary>
    [Required(ErrorMessage = "堆存时间不能为空！")]
    [JsonProperty("dumpTime")]
    public virtual DateTime DumpTime { get; set; } = DateTime.MinValue;

    /// <summary>
    /// 堆存货架，非空
    /// </summary>
    [Required(ErrorMessage = "堆存货架不能为空！")]
    [JsonProperty("shelf")]
    public virtual ShelfInfo Shelf { get; set; } = new ();

    /// <summary>
    /// 堆存库位，非空
    /// </summary>
    [Required(ErrorMessage = "堆存库位不能为空！")]
    [JsonProperty("storageLoc")]
    public virtual StorageLocInfo StorageLoc { get; set; } = new ();

    /// <summary>
    /// 盘点用户，可空
    /// </summary>
    [JsonProperty("stockCheckUser")]
    public virtual UserInfo? StockCheckUser { get; set; } = null;

    /// <summary>
    /// 盘点时间，可空
    /// </summary>
    [JsonProperty("stockCheckTime")]
    public virtual DateTime? StockCheckTime { get; set; } = null;

}