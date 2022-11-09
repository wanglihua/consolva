using System;
using Domain;
using System.Collections.Generic;
using Core;
using Domain.LogisticsOrder;
using Web.Models;

namespace Web.Areas.Admin.Models.LogisticsOrderRefundApprove
{
    public class LogisticsOrderRefundApproveListModel : ListModel
    {
        public LogisticsOrderRefundApproveListModel(ListPager listPager) : base(listPager)
        {
        }

        public IList<LogisticsOrderRefundApplyInfo> LogisticsOrderRefundApplyList { get; set; }
    }
}