using Core;
using Domain.Customer;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.Admin.Models.CustomerScoreLogCustomer
{
    public class CustomerScoreLogCustomerListQueryParam : ListQueryParam
    {
        public int CustomerId { get; set; }

        [ModelBinder(typeof(CustomerScoreTypeModelBinder))]
        public CustomerScoreType ScoreTypeQuery { get; set; }

        public string OrderCodeQuery { get; set; }
    }
}
