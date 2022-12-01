using shunda.Core;
using shunda.Domain.Pass;

namespace shunda.Admin.Areas.Pass.Models.WarehouseWorker;

public class WarehouseWorkerUserListModel : ListModel
{
    public WarehouseWorkerUserListModel(ListPager listPager) : base(listPager)
    {
    }

    public IList<UserInfo> UserList { get; set; } = new List<UserInfo>();
}