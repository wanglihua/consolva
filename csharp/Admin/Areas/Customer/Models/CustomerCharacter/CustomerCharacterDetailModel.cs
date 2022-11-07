using shunda.Domain.Customer;

namespace shunda.Admin.Areas.Customer.Models.CustomerCharacter;

public class CustomerCharacterDetailModel
{
    public CustomerCharacterInfo CustomerCharacter { get; set; } = new();
}