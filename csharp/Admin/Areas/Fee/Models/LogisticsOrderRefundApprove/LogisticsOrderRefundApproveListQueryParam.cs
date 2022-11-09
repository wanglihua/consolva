using System;
using Core;
using Domain.LogisticsOrder;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.Admin.Models.FeeRefundApprove
{
    public class FeeRefundApproveListQueryParam : ListQueryParam
    {
        public int ApplyUserIdQuery { get; set; }
        
        [ModelBinder(typeof(FeeRefundTypeModelBinder))]
        public FeeRefundType TypeQuery { get; set; }

        public string OrderCodeQuery { get; set; }

        public string CustomerCodeQuery { get; set; }

        public string ReasonQuery { get; set; }

        [ModelBinder(typeof(FeeRefundApplyStatusModelBinder))]
        public FeeRefundApplyStatus StatusQuery { get; set; }
    }
}