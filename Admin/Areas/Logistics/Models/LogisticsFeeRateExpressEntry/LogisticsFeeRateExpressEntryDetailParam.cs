namespace shunda.Admin.Areas.Logistics.Models.LogisticsFeeRateExpressEntry;

public class LogisticsFeeRateExpressEntryDetailParam {
    /// <summary>
    /// Id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 快递具体费率，非空
    /// </summary>
    public int FeeRateExpressId { get; set; } = 0;
}