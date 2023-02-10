using shunda.Core;
// using shunda.Core.Util;

namespace shunda.Domain.Wechat;

/// <summary>
/// 微信公众号回调记录
/// </summary>
public class WechatCallbackInfo : EntityNoUpdate {
    /// <summary>
    /// 微信系统类型，非空
    /// </summary>
    public virtual WechatSystemType SystemType { get; set; } = WechatSystemType.None;

    /// <summary>
    /// 回调http方法，非空
    /// </summary>
    public virtual WechatCallbackHttpMethod Method { get; set; } = WechatCallbackHttpMethod.None;

    /// <summary>
    /// 参数，非空
    /// </summary>
    public virtual string Params { get; set; } = string.Empty;

    /// <summary>
    /// body，非空
    /// </summary>
    public virtual string RequestBody { get; set; } = string.Empty;

    /// <summary>
    /// 结果值，非空
    /// </summary>
    public virtual string Result { get; set; } = string.Empty;

    public override void Validate() {
        // ValidationUtil.ValidateEntity<WechatCallbackInfoValidator,  WechatCallbackInfo>(this);
    }
}