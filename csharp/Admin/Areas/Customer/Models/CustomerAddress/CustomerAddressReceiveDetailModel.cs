using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Customer;

namespace shunda.Admin.Areas.Customer.Models.CustomerAddress;

public class CustomerAddressReceiveDetailModel
{
    [BindNever]
    public CustomerReceiveAddressInfo CustomerAddress { get; set; } = new CustomerReceiveAddressInfo();
}