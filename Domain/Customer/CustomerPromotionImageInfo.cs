using shunda.Core;
using shunda.Core.Util;

namespace shunda.Domain.Customer;

/// <summary>
/// 客户推广图片
/// </summary>
public class CustomerPromotionImageInfo : Entity {
    /// <summary>
    /// 图片序号，非空
    /// </summary>
    public virtual int ImageNum { get; set; } = 0;

    /// <summary>
    /// 图片文件名，非空
    /// </summary>
    public virtual string ImageFileName { get; set; } = string.Empty;

    public override void Validate() {
        ValidationUtil.ValidateEntity<CustomerPromotionImageInfoValidator, CustomerPromotionImageInfo>(this);
    }
}