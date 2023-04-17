namespace shunda.Admin.Areas.LogisticsOrder.Models.LogisticsOrderFirstMode;

public class LogisticsOrderFirstModeSaveModel {
    /// <summary>
    /// Id
    /// </summary>
    public int OrderFirstModeId { get; set; }

    /// <summary>
    /// 快递单号，非空
    /// </summary>
    public string TrackingNo { get; set; } = string.Empty;

}