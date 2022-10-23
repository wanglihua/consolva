using shunda.Core;
using shunda.Domain.Customer;

namespace shunda.Admin.Areas.Customer.Models.CustomerCharacter;

public class CustomerCharacterListModel : ListModel
{
    public CustomerCharacterListModel(ListPager listPager) : base(listPager)
    {
    }

    public IList<CustomerCharacterInfo> CustomerCharacterList { get; set; } = new List<CustomerCharacterInfo>();
}