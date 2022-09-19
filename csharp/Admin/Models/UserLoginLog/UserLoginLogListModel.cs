using shunda.Core;
using shunda.Domain.Pass;

namespace shunda.Admin.Models.UserLoginLog;

public class UserLoginLogListModel : ListModel
{
    public UserLoginLogListModel(ListPager listPager) : base(listPager)
    {
    }

    public IList<UserLoginLogInfo> UserLoginLogList { get; set; } = new List<UserLoginLogInfo>();
}