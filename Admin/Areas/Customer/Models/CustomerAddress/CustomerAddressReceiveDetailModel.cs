using shunda.Domain;

namespace shunda.Admin.Areas.Customer.Models.CustomerAddress;

public class CustomerAddressReceiveDetailModel : CustomerAddressReceiveSaveModel {
    public IList<Country> CountryList { get; set; } = new List<Country>();
}