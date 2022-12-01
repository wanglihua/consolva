using shunda.Core;
using shunda.Domain.Pass;

namespace shunda.Admin.Areas.Pass.Models.User;

public class UserListModel : ListModel
{
    public UserListModel(ListPager listPager) : base(listPager)
    {
    }

    public IList<UserInfo> UserList { get; set; } = new List<UserInfo>();
}