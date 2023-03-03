using System.ComponentModel.DataAnnotations;
using shunda.Domain.Client;

namespace shunda.Admin.Areas.Setting.Models.ClientInstruction;

public class ClientInstructionSaveModel {
    public int Id { get; set; }

    /// <summary>
    /// 类型
    /// </summary>
    [Required(ErrorMessage = "类型不能为空！")]
    public ClientInstructionType Type { get; set; } = ClientInstructionType.None;

    /// <summary>
    /// 标题
    /// </summary>
    [Required(ErrorMessage = "标题不能为空！")]
    [MaxLength(200, ErrorMessage = "标题长度不能大于200！")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// URL
    /// </summary>
    [Required(ErrorMessage = "URL不能为空！")]
    [MaxLength(500, ErrorMessage = "URL长度不能大于500！")]
    public string Url { get; set; } = string.Empty;
}