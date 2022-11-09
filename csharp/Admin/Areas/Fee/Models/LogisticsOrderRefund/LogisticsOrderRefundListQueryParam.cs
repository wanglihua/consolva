using System;
using Core;
using Domain.LogisticsOrder;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.Admin.Models.LogisticsOrderRefund
{
    public class LogisticsOrderRefundListQueryParam : ListQueryParam
    {
        [ModelBinder(typeof(FeeRefundTypeModelBinder))]
        public FeeRefundType TypeQuery { get; set; }

        public string OrderCodeQuery { get; set; }

        public string CustomerCodeQuery { get; set; }

        public string ReasonQuery { get; set; }

        [ModelBinder(typeof(FeeRefundStatusModelBinder))]
        public FeeRefundStatus StatusQuery { get; set; }

        public DateTime? BeginDateQuery { get; set; }

        public DateTime? EndDateQuery { get; set; }         
    }
}