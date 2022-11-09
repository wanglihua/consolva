using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Core;
using shunda.Domain.Fee;

namespace shunda.Admin.Areas.Fee.Models.FeeWriteOff;

public class FeeWriteOffListModel : ListModel
{
    public FeeWriteOffListModel(ListPager listPager) : base(listPager)
    {
    }

    [BindNever]
    public IList<LogisticsOrderPayInfo> LogisticsOrderPayList { get; set; } = new List<LogisticsOrderPayInfo>();

    [BindNever]
    public int CustomerCount { get; set; }

    [BindNever]
    public int OrderCount { get; set; }

    [BindNever]
    public decimal AmountSum { get; set; }
}