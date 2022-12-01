using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Logistics;

namespace shunda.Admin.Areas.Logistics.Models.LogisticsMode;

public class LogisticsModeIndexModel
{
    /// <summary>
    ///     物流类型 Id
    /// </summary>
    [BindNever]
    public int TypeId { get; set; }

    /// <summary>
    ///     物流类型
    /// </summary>
    [BindNever]
    public LogisticsModeType Type { get; set; } = LogisticsModeType.None;

    [BindNever]
    public IList<LogisticsModeType> LogisticsModeTypeList { get; set; } = new List<LogisticsModeType>();

    [BindNever]
    public IList<LogisticsModeStatus> StatusList { get; set; } = new List<LogisticsModeStatus>();
}