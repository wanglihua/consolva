using System.Collections.Generic;
using Core;
using Domain.Logistics;

namespace shunda.Admin.Areas.Logistics.Models.LogisticsGoodsTrade;

namespace Web.Areas.Admin.Models.LogisticsGoodsTrade
{
    public class LogisticsGoodsTradeListModel : ListModel
    {
        public LogisticsGoodsTradeListModel(ListPager listPager) : base(listPager)
        {
        }

        public IList<LogisticsGoodsInfo> LogisticsGoodsList { get; set; }
    }
}