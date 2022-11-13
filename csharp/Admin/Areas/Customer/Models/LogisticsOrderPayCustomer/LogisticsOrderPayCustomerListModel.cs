using System.Collections.Generic;
using Core;
using Domain.LogisticsOrder;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Web.Areas.Admin.Models.LogisticsOrderPayCustomer
{
    public class LogisticsOrderPayCustomerListModel : ListModel
    {
        public LogisticsOrderPayCustomerListModel(ListPager listPager) : base(listPager)
        {
        }

        [BindNever]
        public IList<LogisticsOrderPayInfo> LogisticsOrderPayList { get; set; }
    }
}
