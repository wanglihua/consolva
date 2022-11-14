using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Core;
using shunda.Domain.Customer;
using shunda.Domain.Order;
using shunda.Domain.Pass;

namespace shunda.Domain.Express;

/// <summary>
/// 快递单视频
/// </summary>
public class ExpressVideoInfo : Entity
{
    /// <summary>
    /// 客户
    /// </summary>
    [DisplayName("客户")]
    [BindNever]
    public virtual CustomerInfo Customer { get; set; }

    /// <summary>
    /// 物流订单
    /// </summary>
    [DisplayName("物流订单")]
    [BindNever]
    public virtual LogisticsOrderInfo Order { get; set; }

    /// <summary>
    /// 快递单号
    /// </summary>
    [DisplayName("快递单号")]
    [BindNever]
    public virtual ExpressInfo Express { get; set; }

    /// <summary>
    /// 快递单号
    /// </summary>
    [DisplayName("快递单号")]
    public virtual string ExpressTrackingNo { get; set; }

    /// <summary>
    /// 快递公司
    /// </summary>
    [DisplayName("快递公司")]
    public virtual string ExpressComName { get; set; }

    /// <summary>
    /// 类型
    /// </summary>
    [DisplayName("类型")]
    [Required(ErrorMessage = "类型不能为空！")]
    [ModelBinder(typeof(ExpressVideoTypeModelBinder))]
    public virtual ExpressVideoType Type { get; set; }

    /// <summary>
    /// 是否存储在阿里云OSS
    /// </summary>
    public virtual bool? StoreOss { get; set; }

    /// <summary>
    /// 存储路径
    /// </summary>
    [DisplayName("存储路径")]
    [Required(ErrorMessage = "存储路径不能为空！")]
    [MaxLength(255, ErrorMessage = "存储路径长度不能大于255！")]
    public virtual string StorePath { get; set; }

    /// <summary>
    /// 存储名称
    /// </summary>
    [DisplayName("存储名称")]
    [Required(ErrorMessage = "存储名称不能为空！")]
    [MaxLength(50, ErrorMessage = "存储名称长度不能大于50！")]
    public virtual string StoreName { get; set; }

    /// <summary>
    /// 上传用户
    /// </summary>
    public virtual UserInfo User { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    [DisplayName("状态")]
    [ModelBinder(typeof(ExpressVideoStatusModelBinder))]
    public virtual ExpressVideoStatus Status { get; set; }
}