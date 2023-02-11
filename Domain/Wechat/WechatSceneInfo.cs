using shunda.Core;
using shunda.Core.Util;

namespace shunda.Domain.Wechat;

/// <summary>
/// 微信场景
/// </summary>
public class WechatSceneInfo : Entity {
    /// <summary>
    /// 场景Id，非空
    /// </summary>
    public virtual int SceneId { get; set; } = 0;

    /// <summary>
    /// 关联场景类型，非空
    /// </summary>
    public virtual WechatSceneType Type { get; set; } = WechatSceneType.None;

    /// <summary>
    /// 关联Id，非空
    /// </summary>
    public virtual int RelatedId { get; set; } = 0;

    public override void Validate() {
        ValidationUtil.ValidateEntity<WechatSceneInfoValidator,  WechatSceneInfo>(this);
    }
}