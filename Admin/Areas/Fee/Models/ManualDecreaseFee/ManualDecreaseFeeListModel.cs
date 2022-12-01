using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Core;
using shunda.Domain.Fee;

namespace shunda.Admin.Areas.Fee.Models.ManualDecreaseFee;

public class ManualDecreaseFeeListModel : ListModel
{
    public ManualDecreaseFeeListModel(ListPager listPager) : base(listPager)
    {
    }

    [BindNever]
    public IList<LogisticsOrderFeeInfo> LogisticsOrderFeeList { get; set; } = new List<LogisticsOrderFeeInfo>();

    [BindNever]
    public int CustomerCount { get; set; }

    [BindNever]
    public int OrderCount { get; set; }

    [BindNever]
    public decimal FeeSum { get; set; }
}