using Microsoft.AspNetCore.Mvc;
using shunda.Domain.Logistics;

namespace shunda.Admin.Areas.Logistics.Models.LogisticsRepackTypeSetting;

public class LogisticsRepackTypeSettingSaveModel {
    /// <summary>
    /// Id
    /// </summary>
    public int LogisticsRepackTypeSettingId { get; set; }

    /// <summary>
    /// 打包类型，非空
    /// </summary>
    [ModelBinder(typeof(LogisticsRepackTypeModelBinder))]
    public LogisticsRepackType Type { get; set; } = LogisticsRepackType.None;

    /// <summary>
    /// 标题，非空
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 描述，非空
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// 排序，非空
    /// </summary>
    public int OrderNum { get; set; } = 0;

    /// <summary>
    /// 状态，非空
    /// </summary>
    [ModelBinder(typeof(LogisticsRepackTypeStatusModelBinder))]
    public LogisticsRepackTypeStatus Status { get; set; } = LogisticsRepackTypeStatus.None;
}