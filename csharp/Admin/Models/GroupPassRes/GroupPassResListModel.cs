using shunda.Core;

namespace shunda.Admin.Models.GroupPassRes;

public class GroupPassResListModel : ListModel
{
    public GroupPassResListModel(ListPager listPager) : base(listPager)
    {
    }

    public IList<GroupPassResListItem> GroupPassResList { get; set; } = new List<GroupPassResListItem>();

    public string PassResCateAlias { get; set; } = String.Empty;
}