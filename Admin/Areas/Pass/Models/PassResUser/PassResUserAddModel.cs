using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Pass;

namespace shunda.Admin.Areas.Pass.Models.PassResUser;

public class PassResUserAddModel
{
    [Required(ErrorMessage = "资源Id不能为空！")]
    public int PassResId { get; set; }

    [BindNever]
    public PassResInfo? PassRes { get; set; }

    [Required(ErrorMessage = "登录名不能为空！")]
    [MaxLength(100, ErrorMessage = "登录名长度不能大于100！")]
    public string LoginName { get; set; } = string.Empty;
}