using System.Collections.Generic;
using Core;
using Domain.Customer;

namespace Web.Areas.Admin.Models.CustomerAccountEntryCustomer
{
    public class CustomerAccountEntryCustomerListModel : ListModel
    {
        public CustomerAccountEntryCustomerListModel(ListPager listPager) : base(listPager)
        {
        }

        public IList<CustomerAccountEntryInfo> CustomerAccountEntryList { get; set; }
    }
}