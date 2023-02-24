using shunda.Domain.Client;

namespace shunda.Admin.Areas.Setting.Models.ClientNotice;

public class ClientNoticeSaveModel {
    /// <summary>
    /// Id
    /// </summary>
    public int ClientNoticeId { get; set; }

    /// <summary>
    /// 标题，非空
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// URL，非空
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// 编辑者，非空
    /// </summary>
    public int EditorUserId { get; set; } = 0;

    /// <summary>
    /// 是否置顶，非空
    /// </summary>
    public bool ListTop { get; set; } = false;

    /// <summary>
    /// 是否重要公告，非空
    /// </summary>
    public bool Important { get; set; } = false;

}