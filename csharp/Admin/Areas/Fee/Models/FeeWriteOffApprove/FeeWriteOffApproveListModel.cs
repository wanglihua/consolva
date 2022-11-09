using System;
using Domain;
using System.Collections.Generic;
using Core;
using Domain.LogisticsOrder;
using Web.Models;

namespace shunda.Admin.Areas.Fee.Models.LogisticsOrderFeeWriteOffApprove;

namespace Web.Areas.Admin.Models.LogisticsOrderFeeWriteOffApprove
{
    public class LogisticsOrderFeeWriteOffApproveListModel : ListModel
    {
        public LogisticsOrderFeeWriteOffApproveListModel(ListPager listPager) : base(listPager)
        {
        }

        public IList<FeeWriteOffApplyInfo> FeeWriteOffApplyList { get; set; }
    }
}