using shunda.Core;

namespace shunda.Admin.Models.UserPassRes;

public class UserPassResListModel : ListModel
{
    public UserPassResListModel(ListPager listPager) : base(listPager)
    {
    }

    public IList<UserPassResListItem> UserPassResList { get; set; } = new List<UserPassResListItem>();

    public string PassResCateAlias { get; set; } = string.Empty;
}