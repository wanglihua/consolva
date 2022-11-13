using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Customer;

namespace shunda.Admin.Areas.Customer.Models.CustomerAddress;

public class CustomerAddressIndexModel
{
    [BindNever]
    public CustomerInfo Customer { get; set; } = new CustomerInfo();

    [BindNever]
    public IList<CustomerReceiveAddressInfo> CustomerReceiveAddressList { get; set; } = new List<CustomerReceiveAddressInfo>();

    [BindNever]
    public IList<CustomerSendAddressInfo> CustomerSendAddressList { get; set; } = new List<CustomerSendAddressInfo>();    

    [BindNever]
    public int CustomerReceiveAddressCount { get; set; }

    [BindNever]
    public int CustomerSendAddressCount { get; set; }    
}