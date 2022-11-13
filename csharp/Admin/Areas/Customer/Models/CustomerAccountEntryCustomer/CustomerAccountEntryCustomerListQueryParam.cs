using Core;
using Domain.Customer;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.Admin.Models.CustomerAccountEntryCustomer
{
    public class CustomerAccountEntryCustomerListQueryParam : ListQueryParam
    {
        public int CustomerId { get; set; }

        [ModelBinder(typeof(CustomerAccountChangeTypeModelBinder))]
        public CustomerAccountChangeType ChangeTypeQuery { get; set; }
    }
}
