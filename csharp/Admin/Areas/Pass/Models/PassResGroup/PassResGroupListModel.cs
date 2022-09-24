using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Core;
using shunda.Domain.Pass;

namespace shunda.Admin.Areas.Pass.Models.PassResGroup;

public class PassResGroupListModel : ListModel
{
    public PassResGroupListModel(ListPager listPager) : base(listPager)
    {
    }

    [BindNever]
    public IList<GroupPassResInfo> GroupPassResList { get; set; } = new List<GroupPassResInfo>();

    [BindNever]
    public string GroupAlias { get; set; } = string.Empty;
}