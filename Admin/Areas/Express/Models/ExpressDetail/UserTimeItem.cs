using shunda.Domain.Pass;

namespace shunda.Admin.Areas.Express.Models.ExpressDetail;

/// <summary>Express
/// 操作用户，操作时间 组合
/// </summary>
public class UserTimeItem
{
    /// <summary>
    /// 操作用户
    /// </summary>
    public UserInfo User { get; set; } = new ();

    /// <summary>
    /// 操作时间
    /// </summary>
    public DateTime Time { get; set; }
}
