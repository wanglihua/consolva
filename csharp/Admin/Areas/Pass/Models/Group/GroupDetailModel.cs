using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace shunda.Admin.Areas.Pass.Models.Group;

public class GroupDetailModel
{
    public int GroupId { get; set; }

    /// <summary>
    /// 编码
    /// </summary>
    [DisplayName("编码")]
    [Required(ErrorMessage = "编码不能为空！")]
    [MaxLength(50, ErrorMessage = "编码长度不能大于50！")]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 名称
    /// </summary>

    [DisplayName("名称")]
    [Required(ErrorMessage = "名称不能为空！")]
    [MaxLength(50, ErrorMessage = "名称长度不能大于50！")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 描述
    /// </summary>
    [DisplayName("描述")]
    [MaxLength(200, ErrorMessage = "描述长度不能大于200！")]
    public string? Description { get; set; }
}