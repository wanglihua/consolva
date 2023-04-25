using shunda.Core;
using shunda.Core.Util;

namespace shunda.Domain.Client;

/// <summary>
/// 客户端弹窗
/// </summary>
public class ClientPopupInfo : Entity {
    /// <summary>
    /// 是否弹窗，非空
    /// </summary>
    public virtual bool ShouldPopup { get; set; } = false;

    /// <summary>
    /// 内容，非空
    /// </summary>
    public virtual string Content { get; set; } = string.Empty;

    /// <summary>
    /// 编辑时间，非空
    /// </summary>
    public virtual DateTime EditTime { get; set; } = DateTime.MinValue;

    public override void Validate() {
        ValidationUtil.ValidateEntity<ClientPopupInfoValidator, ClientPopupInfo>(this);
    }
}