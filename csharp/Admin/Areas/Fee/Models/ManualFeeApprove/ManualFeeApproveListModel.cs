using System;
using Domain;
using System.Collections.Generic;
using Core;
using Domain.LogisticsOrder;
using Web.Models;

namespace shunda.Admin.Areas.Fee.Models.ManualFeeApprove;

namespace Web.Areas.Admin.Models.ManualFeeApprove
{
    public class ManualFeeApproveListModel : ListModel
    {
        public ManualFeeApproveListModel(ListPager listPager) : base(listPager)
        {
        }

        public IList<ManualFeeApplyInfo> ManualFeeApplyList { get; set; }
    }
}