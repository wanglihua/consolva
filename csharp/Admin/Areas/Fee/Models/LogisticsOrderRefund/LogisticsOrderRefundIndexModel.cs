using System;
using System.Collections.Generic;
using Domain.LogisticsOrder;

namespace Web.Areas.Admin.Models.LogisticsOrderRefund
{
    public class LogisticsOrderRefundIndexModel
    {
        public IList<LogisticsOrderRefundType> TypeList { get; set; }

        public IList<LogisticsOrderRefundStatus> StatusList { get; set; }
    }
}