using System.Collections.Generic;
using Domain.Logistics;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace shunda.Admin.Areas.Logistics.Models.LogisticsGoodsTrade;

namespace Web.Areas.Admin.Models.LogisticsGoodsTrade
{
    public class LogisticsGoodsTradeIndexModel
    {
        [BindNever]
        public int StatusId { get; set; }

        [BindNever]
        public IList<LogisticsGoodsType> GoodsTypeList { get; set; }

        [BindNever]
        public IList<LogisticsGoodsCate> GoodsCateList { get; set; }
    }
}