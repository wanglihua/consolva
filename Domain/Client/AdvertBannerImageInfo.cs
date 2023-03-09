using shunda.Core;
using shunda.Core.Util;

namespace shunda.Domain.Client;

/// <summary>
/// 广告横幅图片
/// </summary>
public class AdvertBannerImageInfo : Entity {
    /// <summary>
    /// 广告横幅，非空
    /// </summary>
    public virtual AdvertBannerInfo Banner { get; set; } = new AdvertBannerInfo();

    /// <summary>
    /// 国家，非空
    /// </summary>
    public virtual int CountryId { get; set; } = 0;

    /// <summary>
    /// 图片序号，非空
    /// </summary>
    public virtual int ImageNum { get; set; } = 0;

    /// <summary>
    /// 存储路径，非空
    /// </summary>
    public virtual string StorePath { get; set; } = string.Empty;

    /// <summary>
    /// 存储名称，非空
    /// </summary>
    public virtual string StoreName { get; set; } = string.Empty;

    /// <summary>
    /// 链接地址，非空
    /// </summary>
    public virtual string LinkUrl { get; set; } = string.Empty;

    public override void Validate() {
        ValidationUtil.ValidateEntity<AdvertBannerImageInfoValidator, AdvertBannerImageInfo>(this);
    }
}