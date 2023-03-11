using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Core;
using shunda.Domain.Express;

namespace shunda.Admin.Areas.Express.Models.ExpressList;

public class ExpressListModel : ListModel {
    public ExpressListModel(ListPager listPager) : base(listPager) {
    }

    [BindNever]
    public IList<ExpressInfo> ExpressList { get; set; } = new List<ExpressInfo>();

    [BindNever]
    public decimal VolumeWeightDenominator { get; set; }
}