using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Core;
using shunda.Domain.Pass;

namespace shunda.Admin.Areas.Pass.Models.PassRes;

public class PassResListModel : ListModel
{
    public PassResListModel(ListPager listPager) : base(listPager)
    {
    }

    [BindNever]
    public IList<PassResInfo> PassResList { get; set; } = new List<PassResInfo>();

    [BindNever]
    public string PassResCateAlias { get; set; } = string.Empty;
}