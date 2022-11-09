using System;
using Core;
using Domain.LogisticsOrder;
using Microsoft.AspNetCore.Mvc;

namespace shunda.Admin.Areas.Fee.Models.ManualFeeApprove;

namespace Web.Areas.Admin.Models.ManualFeeApprove
{
    public class ManualFeeApproveListQueryParam : ListQueryParam
    {
        public int ApplyUserIdQuery { get; set; }

        public int ManualReasonIdQuery { get; set; }

        public string OrderCodeQuery { get; set; }

        public string CustomerCodeQuery { get; set; }

        [ModelBinder(typeof(ManualFeeApplyStatusModelBinder))]
        public ManualFeeApplyStatus StatusQuery { get; set; }
    }
}