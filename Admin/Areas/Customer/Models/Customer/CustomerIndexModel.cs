using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Customer;

namespace shunda.Admin.Areas.Customer.Models.Customer;

public class CustomerIndexModel
{
    [BindNever]
    public IList<Tuple<int, string>> VipLevelList { get; set; } = new List<Tuple<int, string>>();

    [BindNever]
    public IList<CustomerStatus> StatusList { get; set; } = CustomerStatus.GetList();

    [BindNever]
    public IList<CustomerCharacterInfo> CharacterList { get; set; } = new List<CustomerCharacterInfo>();

    [BindNever]
    public bool CanImportTestCustomer { get; set; }
}