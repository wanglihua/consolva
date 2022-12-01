using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Core;

namespace shunda.Domain.Pass;

/// <summary>
/// 群组成员
/// </summary>
public class GroupMemberInfo : EntityNoUpdate
{
    /// <summary>
    /// 群组
    /// </summary>
    // [Required(ErrorMessage = "群组不能为空！")]
    [BindNever]
    public virtual GroupInfo Group { get; set; } = new GroupInfo();

    /// <summary>
    /// 用户
    /// </summary>
    [BindNever]
    public virtual UserInfo User { get; set; } = new UserInfo();
}