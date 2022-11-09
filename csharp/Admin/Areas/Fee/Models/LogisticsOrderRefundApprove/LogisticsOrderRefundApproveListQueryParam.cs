using System;
using Core;
using Domain.LogisticsOrder;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.Admin.Models.LogisticsOrderRefundApprove
{
    public class LogisticsOrderRefundApproveListQueryParam : ListQueryParam
    {
        public int ApplyUserIdQuery { get; set; }
        
        [ModelBinder(typeof(LogisticsOrderRefundTypeModelBinder))]
        public LogisticsOrderRefundType TypeQuery { get; set; }

        public string OrderCodeQuery { get; set; }

        public string CustomerCodeQuery { get; set; }

        public string ReasonQuery { get; set; }

        [ModelBinder(typeof(LogisticsOrderRefundApplyStatusModelBinder))]
        public LogisticsOrderRefundApplyStatus StatusQuery { get; set; }
    }
}