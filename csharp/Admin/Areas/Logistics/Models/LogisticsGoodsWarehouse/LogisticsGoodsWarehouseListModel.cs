using System.Collections.Generic;
using Core;
using Domain.Logistics;

namespace shunda.Admin.Areas.Logistics.Models.LogisticsGoodsWarehouse;

namespace Web.Areas.Admin.Models.LogisticsGoodsWarehouse
{
    public class LogisticsGoodsWarehouseListModel : ListModel
    {
        public LogisticsGoodsWarehouseListModel(ListPager listPager) : base(listPager)
        {
        }

        public IList<LogisticsGoodsInfo> LogisticsGoodsList { get; set; }
    }
}