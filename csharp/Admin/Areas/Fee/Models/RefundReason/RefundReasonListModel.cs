using System;
using Domain;
using System.Collections.Generic;
using Core;
using Domain.LogisticsOrder;
using Web.Models;

namespace Web.Areas.Admin.Models.RefundReason
{
    public class RefundReasonListModel : ListModel
    {
        public RefundReasonListModel(ListPager listPager) : base(listPager)
        {
        }

        public IList<FeeRefundReasonInfo> RefundReasonList { get; set; }
    }
}