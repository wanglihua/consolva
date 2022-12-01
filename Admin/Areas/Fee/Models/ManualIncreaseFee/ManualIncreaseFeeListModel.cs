using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Core;
using shunda.Domain.Fee;

namespace shunda.Admin.Areas.Fee.Models.ManualIncreaseFee;

public class ManualIncreaseFeeListModel : ListModel
{
    public ManualIncreaseFeeListModel(ListPager listPager) : base(listPager)
    {
    }

    [BindNever]
    public IList<LogisticsOrderFeeInfo> LogisticsOrderFeeList { get; set; } = new List<LogisticsOrderFeeInfo>();
}