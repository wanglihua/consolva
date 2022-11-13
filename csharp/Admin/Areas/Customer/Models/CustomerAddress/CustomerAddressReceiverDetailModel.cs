using System.Collections.Generic;
using Domain;
using Domain.Customer;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Web.Areas.Admin.Models.CustomerAddress
{
    public class CustomerAddressReceiverDetailModel
    {
        public CustomerAddressInfo CustomerAddress { get; set; }

        [BindNever]
        public IList<AustraliaState> AustraliaStateList { get; set; }
    }
}