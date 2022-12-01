using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Core;
using shunda.Domain.Logistics;

namespace shunda.Admin.Areas.Logistics.Models.LogisticsModeInner;

public class LogisticsModeInnerListModel : ListModel
{
    public LogisticsModeInnerListModel(ListPager listPager) : base(listPager)
    {
    }

    [BindNever]
    public IList<LogisticsModeInfo> LogisticsModeList { get; set; } = new List<LogisticsModeInfo>();

    [BindNever]
    public LogisticsModeType ModeType { get; set; } = LogisticsModeType.None;
}