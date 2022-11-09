using System;
using Domain;
using System.Collections.Generic;
using Core;
using Domain.LogisticsOrder;
using Web.Models;

namespace shunda.Admin.Areas.Fee.Models.ManualFeeApply;

namespace Web.Areas.Admin.Models.ManualFeeApply
{
    public class ManualFeeApplyListModel : ListModel
    {
        public ManualFeeApplyListModel(ListPager listPager) : base(listPager)
        {
        }

        public IList<ManualFeeApplyInfo> ManualFeeApplyList { get; set; }
    }
}