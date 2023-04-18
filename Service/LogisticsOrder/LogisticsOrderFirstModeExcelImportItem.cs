using shunda.Domain.LogisticsOrder;

namespace shunda.Service.LogisticsOrder;

public class LogisticsOrderFirstModeExcelImportItem {
    public int OrderFirstModeId { get; set; }

    // 数据库查询结果暂存
    public LogisticsOrderFirstModeInfo OrderFirstMode { get; set; } = new();

    public int OrderId { get; set; }

    public int FirstModeId { get; set; }

    public string OrderCode { get; set; } = string.Empty;

    public string TrackingNo { get; set; } = string.Empty;
}