using System.Collections.Generic;
using Core;
using Domain.Logistics;

namespace shunda.Admin.Areas.Logistics.Models.LogisticsGoods;

namespace Web.Areas.Admin.Models.LogisticsGoods
{
    public class LogisticsGoodsListModel : ListModel
    {
        public LogisticsGoodsListModel(ListPager listPager) : base(listPager)
        {
        }

        public IList<LogisticsGoodsInfo> LogisticsGoodsList { get; set; }
    }
}