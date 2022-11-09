using System;
using Domain;
using System.Collections.Generic;
using Core;
using Domain.LogisticsOrder;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Web.Models;

namespace Web.Areas.Admin.Models.LogisticsOrderRefund
{
    public class LogisticsOrderRefundListModel : ListModel
    {
        public LogisticsOrderRefundListModel(ListPager listPager) : base(listPager)
        {
        }

        [BindNever]
        public IList<LogisticsOrderRefundInfo> LogisticsOrderRefundList { get; set; }

        [BindNever]
        public int CustomerCount { get; set; }

        [BindNever]
        public int OrderCount { get; set; }

        [BindNever]
        public decimal AmountSum { get; set; }        
    }
}