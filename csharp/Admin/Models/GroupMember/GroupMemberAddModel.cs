using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Pass;

namespace shunda.Admin.Models.GroupMember;

public class GroupMemberAddModel
{
    [Required(ErrorMessage = "群组Id不能为空！")]
    public int GroupId { get; set; }

    /// <summary>
    /// 用户在成员添加页面显示当前群组的一些信息
    /// </summary>
    [BindNever]
    public GroupInfo? Group { get; set; }

    /// <summary>
    /// 要添加到群组的用户的登录名
    /// </summary>
    [Required(ErrorMessage = "登录名不能为空！")]
    [MaxLength(100, ErrorMessage = "登录名长度不能大于100！")]
    public string LoginName { get; set; } = string.Empty;
}