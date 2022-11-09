using System;
using System.Collections.Generic;
using Domain;
using Domain.LogisticsOrder;

namespace Web.Areas.Admin.Models.RefundReason
{
    public class RefundReasonDetailModel
    {
        public RefundReasonInfo RefundReason { get; set; }

        public IList<RefundReasonStatus> StatusList { get; set; }
    }
}