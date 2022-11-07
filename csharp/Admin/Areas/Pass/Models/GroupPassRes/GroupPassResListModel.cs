using shunda.Core;

namespace shunda.Admin.Areas.Pass.Models.GroupPassRes;

public class GroupPassResListModel : ListModel
{
    public GroupPassResListModel(ListPager listPager) : base(listPager)
    {
    }

    public IList<GroupPassResListItem> GroupPassResList { get; set; } = new List<GroupPassResListItem>();

    public string PassResCateAlias { get; set; } = string.Empty;
}