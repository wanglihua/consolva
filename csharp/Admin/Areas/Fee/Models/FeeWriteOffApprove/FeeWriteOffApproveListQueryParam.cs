using System;
using Core;
using Domain.LogisticsOrder;
using Microsoft.AspNetCore.Mvc;

namespace shunda.Admin.Areas.Fee.Models.LogisticsOrderFeeWriteOffApprove;

namespace Web.Areas.Admin.Models.LogisticsOrderFeeWriteOffApprove
{
    public class LogisticsOrderFeeWriteOffApproveListQueryParam : ListQueryParam
    {
        public int ApplyUserIdQuery { get; set; }
        
        public int ManualReasonIdQuery { get; set; }

        public string OrderCodeQuery { get; set; }

        public string CustomerCodeQuery { get; set; }

        [ModelBinder(typeof(FeeWriteOffApplyStatusModelBinder))]
        public FeeWriteOffApplyStatus StatusQuery { get; set; }
    }
}