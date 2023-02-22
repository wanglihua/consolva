using shunda.Core;
using shunda.Domain.Logistics;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace shunda.Admin.Areas.Logistics.Models.LogisticsFeeRateExpressEntry;

public class LogisticsFeeRateExpressEntryListModel : ListModel {
    public LogisticsFeeRateExpressEntryListModel(ListPager listPager) : base(listPager) {
    }

    public string LogisticsFeeRateExpressEntryAlias = string.Empty;
    public string FeeRateExpressAlias { get; set; } = string.Empty;    

    [BindNever]
    public IList<LogisticsFeeRateExpressEntryInfo> LogisticsFeeRateExpressEntryList { get; set; } = new List<LogisticsFeeRateExpressEntryInfo>();
}