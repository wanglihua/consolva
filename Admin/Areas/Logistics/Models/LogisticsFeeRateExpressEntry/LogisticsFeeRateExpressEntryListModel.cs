using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Logistics;

namespace shunda.Admin.Areas.Logistics.Models.LogisticsFeeRateExpressEntry;

public class LogisticsFeeRateExpressEntryListModel {
    [BindNever]
    public string FeeRateExpressEntryAlias { get; set; } = string.Empty;

    [BindNever]
    public string FeeRateExpressAlias { get; set; } = string.Empty;

    [BindNever]
    public IList<LogisticsFeeRateExpressEntryInfo> EntryList { get; set; } = new List<LogisticsFeeRateExpressEntryInfo>();
}