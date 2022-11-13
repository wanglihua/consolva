using System.Collections.Generic;
using Core;
using Domain.Customer;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Web.Areas.Admin.Models.CustomerScoreLogCustomer
{
    public class CustomerScoreLogCustomerListModel : ListModel
    {
        public CustomerScoreLogCustomerListModel(ListPager listPager) : base(listPager)
        {
        }

        [BindNever]
        public IList<CustomerScoreLogInfo> CustomerScoreLogList { get; set; }
    }
}