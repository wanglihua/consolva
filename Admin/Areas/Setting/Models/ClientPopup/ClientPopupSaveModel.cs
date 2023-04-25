using shunda.Domain.Client;

namespace shunda.Admin.Areas.Setting.Models.ClientPopup;

public class ClientPopupSaveModel {
    /// <summary>
    /// Id
    /// </summary>
    public int ClientPopupId { get; set; }

    /// <summary>
    /// 是否弹窗，非空
    /// </summary>
    public bool ShouldPopup { get; set; } = false;

    /// <summary>
    /// 内容，非空
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 编辑时间，非空
    /// </summary>
    public DateTime EditTime { get; set; } = DateTime.MinValue;

}