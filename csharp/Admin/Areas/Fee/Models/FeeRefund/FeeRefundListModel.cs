using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Core;
using shunda.Domain.Fee;

namespace shunda.Admin.Areas.Fee.Models.FeeRefund;

public class FeeRefundListModel : ListModel
{
    public FeeRefundListModel(ListPager listPager) : base(listPager)
    {
    }

    [BindNever]
    public IList<FeeRefundInfo> FeeRefundList { get; set; } = new List<FeeRefundInfo>();

    [BindNever]
    public int CustomerCount { get; set; }

    [BindNever]
    public int OrderCount { get; set; }

    [BindNever]
    public decimal AmountSum { get; set; }
}