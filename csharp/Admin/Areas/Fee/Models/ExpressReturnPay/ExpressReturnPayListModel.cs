using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Core;
using shunda.Domain.Fee;

namespace shunda.Admin.Areas.Fee.Models.ExpressReturnPay;

public class ExpressReturnPayListModel : ListModel
{
    public ExpressReturnPayListModel(ListPager listPager) : base(listPager)
    {
    }

    [BindNever]
    public IList<ExpressReturnPayInfo> ExpressReturnPayList { get; set; } = new List<ExpressReturnPayInfo>();
}