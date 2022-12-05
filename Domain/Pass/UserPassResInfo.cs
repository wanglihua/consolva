using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using shunda.Core;

namespace shunda.Domain.Pass;

/// <summary>
/// 用户资源授权
/// </summary>
public class UserPassResInfo : EntityNoUpdate
{
    /// <summary>
    /// 用户
    /// </summary>
    [Required(ErrorMessage = "用户不能为空！")]
    public virtual UserInfo User { get; set; } = new ();

    /// <summary>
    /// 资源
    /// </summary>
    [Required(ErrorMessage = "资源不能为空！")]
    public virtual PassResInfo PassRes { get; set; } = new ();
}