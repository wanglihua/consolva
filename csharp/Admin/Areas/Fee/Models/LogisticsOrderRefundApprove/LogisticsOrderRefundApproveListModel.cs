using System;
using Domain;
using System.Collections.Generic;
using Core;
using Domain.LogisticsOrder;
using Web.Models;

namespace Web.Areas.Admin.Models.FeeRefundApprove
{
    public class FeeRefundApproveListModel : ListModel
    {
        public FeeRefundApproveListModel(ListPager listPager) : base(listPager)
        {
        }

        public IList<FeeRefundApplyInfo> FeeRefundApplyList { get; set; }
    }
}