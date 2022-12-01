using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Pass;

namespace shunda.Admin.Areas.Pass.Models.PassResGroup;

public class PassResGroupAddModel
{
    [Required(ErrorMessage = "资源Id不能为空！")]
    public int PassResId { get; set; }

    [BindNever]
    public PassResInfo? PassRes { get; set; }

    [Required(ErrorMessage = "群组编码不能为空！")]
    [MaxLength(50, ErrorMessage = "群组编码长度不能大于50！")]
    public string GroupCode { get; set; } = string.Empty;
}