using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using shunda.Core;

namespace shunda.Domain.Customer;

/// <summary>
/// 客户性质
/// </summary>
public class CustomerCharacterInfo : Entity
{
    /// <summary>
    /// 内容
    /// </summary>
    [DisplayName("内容")]
    [Required(ErrorMessage = "内容不能为空！")]
    [MaxLength(5, ErrorMessage = "内容长度不能大于5！")]
    public virtual string Content { get; set; } = string.Empty;
}