namespace shunda.Admin.Areas.Pass.Models.Group;

public class GroupSaveModel {
    public int GroupId { get; set; }

    /// <summary>
    /// 编码
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 描述
    /// </summary>
    public string? Description { get; set; }
}
