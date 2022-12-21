using System.ComponentModel.DataAnnotations;

namespace shunda.Admin.Areas.Pass.Models.Admin;

public class AdminAddModel
{
    /// <summary>
    /// 登录名
    /// </summary>
    [Required(ErrorMessage = "登录名不能为空！")]
    [MaxLength(100, ErrorMessage = "登录名长度不能大于100！")]
    public string LoginName { get; set; } = string.Empty;
}