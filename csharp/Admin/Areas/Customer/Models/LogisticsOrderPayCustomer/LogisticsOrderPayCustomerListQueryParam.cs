using Core;

namespace Web.Areas.Admin.Models.LogisticsOrderPayCustomer
{
    public class LogisticsOrderPayCustomerListQueryParam : ListQueryParam
    {
        public int CustomerId { get; set; }

        public string OrderCodeQuery { get; set; }
    }
}