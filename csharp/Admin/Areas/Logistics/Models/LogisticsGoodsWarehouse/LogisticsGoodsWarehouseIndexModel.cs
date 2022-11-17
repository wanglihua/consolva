using System.Collections.Generic;
using Domain.Logistics;

namespace shunda.Admin.Areas.Logistics.Models.LogisticsGoodsWarehouse;

namespace Web.Areas.Admin.Models.LogisticsGoodsWarehouse
{
    public class LogisticsGoodsWarehouseIndexModel
    {
        // public IList<LogisticsGoodsType> GoodsTypeList { get; set; }

        public IList<LogisticsGoodsCate> GoodsCateList { get; set; }
    }
}