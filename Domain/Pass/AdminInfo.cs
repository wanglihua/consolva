using System.ComponentModel;
using shunda.Core;

namespace shunda.Domain.Pass;

/// <summary>
/// 管理员
/// </summary>
public class AdminInfo : Entity
{
    /// <summary>
    /// 内置超级管理员的登录名
    /// </summary>
    public static readonly string SupperAdminLoginName = "admin";

    /// <summary>
    /// 用户
    /// </summary>
    public virtual UserInfo User { get; set; } = new UserInfo();
}