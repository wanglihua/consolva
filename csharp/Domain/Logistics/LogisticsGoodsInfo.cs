using System.ComponentModel.DataAnnotations;
using shunda.Core;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using shunda.Domain.Express;
using shunda.Domain.Pass;

namespace shunda.Domain.Logistics;

/// <summary>
/// 物流物品
/// </summary>
public class LogisticsGoodsInfo : Entity
{
    /// <summary>
    /// 扩展名，非空
    /// </summary>
    [Required(ErrorMessage = "扩展名不能为空！")]
    [MaxLength(200, ErrorMessage = "扩展名长度不能大于200！")]
    [JsonProperty("name")]
    public virtual string Name { get; set; } = string.Empty;

    /// <summary>
    /// 类别，非空
    /// </summary>
    [Required(ErrorMessage = "类别不能为空！")]
    [JsonProperty("cate")]
    [ModelBinder(typeof(LogisticsGoodsCateModelBinder))]
    public virtual LogisticsGoodsCate Cate { get; set; } = LogisticsGoodsCate.None;

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
    /// 海关品名，可空
    /// </summary>
    [JsonProperty("hsGoods")]
    public virtual HsGoodsInfo? HsGoods { get; set; } = null;

    /// <summary>
    /// 添加人员，非空
    /// </summary>
    [Required(ErrorMessage = "添加人员不能为空！")]
    [JsonProperty("addUser")]
    public virtual UserInfo AddUser { get; set; } = new UserInfo();

    /// <summary>
    /// 最后修改人，非空
    /// </summary>
    [Required(ErrorMessage = "最后修改人不能为空！")]
    [JsonProperty("updateUser")]
    public virtual UserInfo UpdateUser { get; set; } = new UserInfo();

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

    #region cache

    public virtual IList<ExpressPhotoInfo> CreateExpressPhotoListCache { get; set; } = new List<ExpressPhotoInfo>();

    public virtual IList<ExpressPhotoInfo> FirstOrderExpressPhotoListCache { get; set; } = new List<ExpressPhotoInfo>();

    public virtual IList<ExpressPhotoInfo> SecondOrderExpressPhotoListCache { get; set; } = new List<ExpressPhotoInfo>();

    public virtual IList<ExpressPhotoInfo> ThirdOrderExpressPhotoListCache { get; set; } = new List<ExpressPhotoInfo>();

    #endregion
}