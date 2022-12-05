using shunda.Core;
using shunda.Domain.Storage;

namespace shunda.Domain.Express;

/// <summary>
/// 自动秤设备
/// </summary>
public class AutoMeasureMachineInfo : Entity
{
    /// <summary>
    /// 名称，非空
    /// </summary>
    public virtual string Name { get; set; } = string.Empty;

    /// <summary>
    /// 描述，非空
    /// </summary>
    public virtual string Description { get; set; } = string.Empty;

    /// <summary>
    /// 仓库，非空
    /// </summary>
    public virtual WarehouseInfo Warehouse { get; set; } = new ();

    /// <summary>
    /// app key，非空
    /// </summary>
    public virtual string AppKey { get; set; } = string.Empty;

    /// <summary>
    /// secret，非空
    /// </summary>
    public virtual string Secret { get; set; } = string.Empty;
}