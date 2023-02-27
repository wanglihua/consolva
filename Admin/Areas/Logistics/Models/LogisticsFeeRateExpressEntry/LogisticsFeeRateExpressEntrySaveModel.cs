namespace shunda.Admin.Areas.Logistics.Models.LogisticsFeeRateExpressEntry;

public class LogisticsFeeRateExpressEntrySaveModel {
    /// <summary>
    /// Id
    /// </summary>
    public int FeeRateExpressEntryId { get; set; }

    /// <summary>
    /// 快递具体费率，非空
    /// </summary>
    public int FeeRateExpressId { get; set; } = 0;

    /// <summary>
    /// 开始重量，非空
    /// </summary>
    public decimal BeginWeight { get; set; } = 0m;

    /// <summary>
    /// 结束重量，非空
    /// </summary>
    public decimal EndWeight { get; set; } = 0m;

    /// <summary>
    /// 每公斤费率，非空
    /// </summary>
    public decimal FeePerKilogram { get; set; }

}