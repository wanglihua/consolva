using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Core;
using shunda.Domain;
using shunda.Domain.Logistics;

namespace shunda.Admin.Areas.Logistics.Models.LogisticsMode;

public class LogisticsModeListModel : ListModel {
    public LogisticsModeListModel(ListPager listPager) : base(listPager) {
    }

    [BindNever]
    public IList<LogisticsModeInfo> LogisticsModeList { get; set; } = new List<LogisticsModeInfo>();

    [BindNever]
    public Country Country { get; set; } = Country.None;
}