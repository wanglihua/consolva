using System;
using Domain;
using System.Collections.Generic;
using Core;
using Web.Models;

namespace Web.Areas.Admin.Models.HsGoods
{
    public class HsGoodsListModel : ListModel
    {
        public HsGoodsListModel(ListPager listPager) : base(listPager)
        {
        }

        public IList<HsGoodsInfo> HsGoodsList { get; set; }
    }
}