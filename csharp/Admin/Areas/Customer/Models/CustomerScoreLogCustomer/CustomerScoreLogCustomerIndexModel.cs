using System.Collections.Generic;
using Domain.Customer;

namespace Web.Areas.Admin.Models.CustomerScoreLogCustomer
{
    public class CustomerScoreLogCustomerIndexModel
    {
        public int CustomerId { get; set; }

        public CustomerInfo Customer { get; set; }

        public IList<CustomerScoreType> ScoreTypeList { get; set; }
    }
}