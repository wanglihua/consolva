using System;
using System.Collections.Generic;
using Domain.LogisticsOrder;

namespace Web.Areas.Admin.Models.LogisticsOrderRefund
{
    public class LogisticsOrderRefundIndexModel
    {
        public IList<FeeRefundType> TypeList { get; set; }

        public IList<FeeRefundStatus> StatusList { get; set; }
    }
}