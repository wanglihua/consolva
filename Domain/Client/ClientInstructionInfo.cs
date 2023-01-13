using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Core;
using shunda.Domain.Pass;

namespace shunda.Domain.Client;

/// <summary>
/// 客户端说明
/// </summary>
public class ClientInstructionInfo : Entity {
    /// <summary>
    /// 类型
    /// </summary>
    public virtual ClientInstructionType Type { get; set; } = ClientInstructionType.None;

    /// <summary>
    /// 标题
    /// </summary>
    [DisplayName("标题")]
    [Required(ErrorMessage = "标题不能为空！")]
    [MaxLength(200, ErrorMessage = "标题长度不能大于200！")]
    public virtual string Title { get; set; } = string.Empty;

    /// <summary>
    /// 内容
    /// </summary>
    [DisplayName("内容")]
    public virtual string Content { get; set; } = string.Empty;

    /// <summary>
    /// 添加人
    /// </summary>
    [DisplayName("添加人")]
    // [Required(ErrorMessage = "添加人不能为空！")]
    [BindNever]
    public virtual UserInfo Creator { get; set; } = new();

    /// <summary>
    /// 发布时间
    /// </summary>
    [DisplayName("发布时间")]
    public virtual DateTime? PublishTime { get; set; }
}