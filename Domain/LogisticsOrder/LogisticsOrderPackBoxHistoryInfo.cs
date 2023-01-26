using Microsoft.AspNetCore.Mvc;
using shunda.Core;
using shunda.Domain.Pass;

namespace shunda.Domain.LogisticsOrder;

/// <summary>
/// 订单打包箱号历史
/// </summary>
public class LogisticsOrderPackBoxHistoryInfo : EntityNoUpdate {
    [ModelBinder(typeof(LogisticsOrderPackBoxHistoryTypeModelBinder))]
    public virtual LogisticsOrderPackBoxHistoryType Type { get; set; } = LogisticsOrderPackBoxHistoryType.None;

    /// <summary>
    /// 订单
    /// </summary>
    public virtual LogisticsOrderInfo Order { get; set; } = new();

    /// <summary>
    /// 箱子
    /// </summary>
    public virtual LogisticsOrderPackBoxInfo OrderPackBox { get; set; } = new();

    /// <summary>
    /// 箱序
    /// </summary>
    public virtual int OrderPackBoxNum { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    [ModelBinder(typeof(LogisticsOrderPackBoxStatusModelBinder))]
    public virtual LogisticsOrderPackBoxStatus StatusBefore { get; set; } = LogisticsOrderPackBoxStatus.None;

    /// <summary>
    /// 状态
    /// </summary>
    [ModelBinder(typeof(LogisticsOrderPackBoxStatusModelBinder))]
    public virtual LogisticsOrderPackBoxStatus StatusAfter { get; set; } = LogisticsOrderPackBoxStatus.None;

    /// <summary>
    /// 操作用户
    /// </summary>
    public virtual UserInfo User { get; set; } = new();

    /// <summary>
    /// 描述
    /// </summary>
    public virtual string Description { get; set; } = string.Empty;
}