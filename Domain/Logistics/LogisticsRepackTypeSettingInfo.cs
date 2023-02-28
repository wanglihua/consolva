using shunda.Core;
using shunda.Core.Util;

namespace shunda.Domain.Logistics;

/// <summary>
/// 打包类型设置
/// </summary>
public class LogisticsRepackTypeSettingInfo : Entity {
    /// <summary>
    /// 打包类型，非空
    /// </summary>
    public virtual LogisticsRepackType Type { get; set; } = LogisticsRepackType.None;

    /// <summary>
    /// 标题，非空
    /// </summary>
    public virtual string Title { get; set; } = string.Empty;

    /// <summary>
    /// 描述，非空
    /// </summary>
    public virtual string Description { get; set; } = string.Empty;

    /// <summary>
    /// 排序，非空
    /// </summary>
    public virtual int OrderNum { get; set; } = 0;

    /// <summary>
    /// 状态，非空
    /// </summary>
    public virtual LogisticsRepackTypeStatus Status { get; set; } = LogisticsRepackTypeStatus.None;

    public override void Validate() {
        ValidationUtil.ValidateEntity<LogisticsRepackTypeSettingInfoValidator, LogisticsRepackTypeSettingInfo>(this);
    }
}