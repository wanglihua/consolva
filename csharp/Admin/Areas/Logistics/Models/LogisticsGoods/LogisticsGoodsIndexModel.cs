using System;
using System.Collections.Generic;
using Domain.Logistics;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace shunda.Admin.Areas.Logistics.Models.LogisticsGoods;

namespace Web.Areas.Admin.Models.LogisticsGoods
{
    public class LogisticsGoodsIndexModel
    {
        [BindNever]
        public LogisticsGoodsType Type { get; set; }

        public IList<LogisticsGoodsCate> GoodsCateList { get; set; }
    }
}