using shunda.Domain.Pass;

namespace shunda.Admin.Areas.Express.Models.ExpressClaim;

/// <summary>
/// 操作用户，操作时间 组合
/// </summary>
public class UserTimeItem
{
    /// <summary>
    /// 操作用户
    /// </summary>
    public UserInfo User { get; set; } = new UserInfo();

    /// <summary>
    /// 操作时间
    /// </summary>
    public DateTime Time { get; set; }
}

public class LoadUnloadUserTime
{
    /// <summary>
    /// 装箱，操作用户和时间
    /// </summary>
    public UserTimeItem LoadUserTime { get; set; } = new UserTimeItem();

    /// <summary>
    /// 卸货，操作用户和时间
    /// </summary>
    public UserTimeItem UnloadUserTime { get; set; } = new UserTimeItem();
}