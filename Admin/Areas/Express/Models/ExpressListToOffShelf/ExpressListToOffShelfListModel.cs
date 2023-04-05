using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Core;
using shunda.Domain.Express;

namespace shunda.Admin.Areas.Express.Models.ExpressListToOffShelf;

public class ExpressListToOffShelfListModel : ListModel {
    public ExpressListToOffShelfListModel(ListPager listPager) : base(listPager) {
    }

    [BindNever]
    public IList<ExpressInfo> ExpressList { get; set; } = new List<ExpressInfo>();

    [BindNever]
    public decimal VolumeWeightDenominator { get; set; }
}
