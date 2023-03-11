using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Core;
using shunda.Domain.Express;

namespace shunda.Admin.Areas.Express.Models.ExpressSizeWeight;

public class ExpressSizeWeightListModel : ListModel {
    public ExpressSizeWeightListModel(ListPager listPager) : base(listPager) {
    }

    [BindNever]
    public IList<ExpressSizeWeightInfo> ExpressSizeWeightList { get; set; } = new List<ExpressSizeWeightInfo>();

    [BindNever]
    public decimal VolumeWeightDenominator { get; set; }
}