using System;
using Domain;
using System.Collections.Generic;
using Core;
using Domain.LogisticsOrder;
using Web.Models;

namespace Web.Areas.Admin.Models.LogisticsOrderRefundApply
{
    public class LogisticsOrderRefundApplyListModel : ListModel
    {
        public LogisticsOrderRefundApplyListModel(ListPager listPager) : base(listPager)
        {
        }

        public IList<LogisticsOrderRefundApplyInfo> LogisticsOrderRefundApplyList { get; set; }
    }
}