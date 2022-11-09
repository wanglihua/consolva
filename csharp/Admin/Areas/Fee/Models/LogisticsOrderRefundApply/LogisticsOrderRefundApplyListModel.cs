using System;
using Domain;
using System.Collections.Generic;
using Core;
using Domain.LogisticsOrder;
using Web.Models;

namespace Web.Areas.Admin.Models.FeeRefundApply
{
    public class FeeRefundApplyListModel : ListModel
    {
        public FeeRefundApplyListModel(ListPager listPager) : base(listPager)
        {
        }

        public IList<FeeRefundApplyInfo> FeeRefundApplyList { get; set; }
    }
}