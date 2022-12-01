using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using shunda.Core;

namespace shunda.Domain.Customer;

/// <summary>
/// 客户超级密码
/// </summary>
public class CustomerSupperPasswordInfo : Entity
{
    /// <summary>
    /// 客户id
    /// </summary>
    [Required(ErrorMessage = "客户id不能为空！")]
    public virtual int CustomerId { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    [Required(ErrorMessage = "密码不能为空！")]
    [MaxLength(40, ErrorMessage = "密码长度不能大于40！")]
    public virtual string Password { get; set; } = string.Empty;
}