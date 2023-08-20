using System.ComponentModel.DataAnnotations;
using shunda.Core;

namespace shunda.Domain.Customer;

/// <summary>
/// 客户推广图片描述
/// </summary>
public class CustomerPromotionImageSettingInfo : Entity {
    /// <summary>
    /// 是否在客户端展现
    /// </summary>
    public virtual bool ClientShow { get; set; } = false;

    /// <summary>
    /// 推广图片描述
    /// </summary>
    [Required(ErrorMessage = "推广图片描述不能为空！")]
    [MaxLength(10000, ErrorMessage = "推广图片描述不能大于10000！")]
    public virtual string ImageDesc { get; set; } = string.Empty;
}