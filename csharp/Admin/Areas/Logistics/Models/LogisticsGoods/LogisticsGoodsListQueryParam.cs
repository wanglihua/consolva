using System;
using Core;
using Domain.Logistics;
using Microsoft.AspNetCore.Mvc;

namespace shunda.Admin.Areas.Logistics.Models.LogisticsGoods;

namespace Web.Areas.Admin.Models.LogisticsGoods
{
    public class LogisticsGoodsListQueryParam : ListQueryParam
    {
        [ModelBinder(typeof(LogisticsGoodsTypeModelBinder))]
        public LogisticsGoodsType TypeQuery { get; set; }

        [ModelBinder(typeof(LogisticsGoodsCateModelBinder))]
        public LogisticsGoodsCate CateQuery { get; set; }

        public string NameQuery { get; set; }

        public string CnHsCodeQuery { get; set; }

        public string CnHsNameQuery { get; set; }

        public string AuHsCodeQuery { get; set; }

        public string AuHsNameQuery { get; set; }
    }
}