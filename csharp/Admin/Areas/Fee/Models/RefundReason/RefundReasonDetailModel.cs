using System;
using System.Collections.Generic;
using Domain;
using Domain.LogisticsOrder;

namespace Web.Areas.Admin.Models.RefundReason
{
    public class RefundReasonDetailModel
    {
        public FeeRefundReasonInfo RefundReason { get; set; }

        public IList<FeeRefundReasonStatus> StatusList { get; set; }
    }
}