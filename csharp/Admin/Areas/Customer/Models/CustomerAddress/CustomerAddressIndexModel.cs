using System.Collections.Generic;
using Domain.Customer;

namespace Web.Areas.Admin.Models.CustomerAddress
{
    public class CustomerAddressIndexModel
    {
        public CustomerInfo Customer { get; set; }

        public IList<CustomerAddressInfo> CustomerAddressList { get; set; }

        public int CustomerAddressCount { get; set; }
    }
}