using System.ComponentModel;

namespace shunda.Domain.Warehouse;

/// <summary>
/// 仓库带有地址描述信息
/// </summary>
public class WarehouseWithAddressDescriptionInfo : WarehouseInfo
{
    /// <summary>
    /// 地址描述
    /// </summary>
    [DisplayName("地址描述")]
    public virtual string AddressDescription { get; set; } = string.Empty;
}