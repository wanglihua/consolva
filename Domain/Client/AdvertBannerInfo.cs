using shunda.Core;
using shunda.Core.Util;

namespace shunda.Domain.Client;

/// <summary>
/// 广告横幅
/// </summary>
public class AdvertBannerInfo : Entity {
    /// <summary>
    /// 位置，非空
    /// </summary>
    public virtual AdvertBannerLoc Loc { get; set; } = AdvertBannerLoc.None;

    /// <summary>
    /// 名称，非空
    /// </summary>
    public virtual string Name { get; set; } = string.Empty;

    /// <summary>
    /// 是否客户端显示，非空
    /// </summary>
    public virtual bool ClientShow { get; set; } = false;

    public override void Validate() {
        ValidationUtil.ValidateEntity<AdvertBannerInfoValidator, AdvertBannerInfo>(this);
    }
}