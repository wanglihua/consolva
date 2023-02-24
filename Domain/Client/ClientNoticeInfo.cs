using shunda.Core;
using shunda.Core.Util;
using shunda.Domain.Pass;

namespace shunda.Domain.Client;

/// <summary>
/// 客户端公告
/// </summary>
public class ClientNoticeInfo : Entity {
    /// <summary>
    /// 标题，非空
    /// </summary>
    public virtual string Title { get; set; } = string.Empty;

    /// <summary>
    /// URL，非空
    /// </summary>
    public virtual string Url { get; set; } = string.Empty;

    /// <summary>
    /// 编辑者，非空
    /// </summary>
    public virtual UserInfo EditorUser { get; set; } = new UserInfo();

    /// <summary>
    /// 是否置顶，非空
    /// </summary>
    public virtual bool ListTop { get; set; } = false;

    /// <summary>
    /// 是否重要公告，非空
    /// </summary>
    public virtual bool Important { get; set; } = false;

    public override void Validate() {
        ValidationUtil.ValidateEntity<ClientNoticeInfoValidator, ClientNoticeInfo>(this);
    }
}