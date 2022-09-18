using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Core;
using shunda.Domain.Pass;

namespace shunda.Admin.Models.PassResUser;

public class PassResUserListModel : ListModel
{
    public PassResUserListModel(ListPager listPager) : base(listPager)
    {
    }

    [BindNever]
    public IList<UserPassResInfo> UserPassResList { get; set; } = new List<UserPassResInfo>();

    [BindNever]
    public string UserAlias { get; set; } = string.Empty;
}