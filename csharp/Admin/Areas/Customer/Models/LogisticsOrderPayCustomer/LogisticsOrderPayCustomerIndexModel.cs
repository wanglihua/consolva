using Domain.Customer;

namespace Web.Areas.Admin.Models.LogisticsOrderPayCustomer
{
    public class LogisticsOrderPayCustomerIndexModel
    {
        public int CustomerId { get; set; }

        public CustomerInfo Customer { get; set; }
    }
}