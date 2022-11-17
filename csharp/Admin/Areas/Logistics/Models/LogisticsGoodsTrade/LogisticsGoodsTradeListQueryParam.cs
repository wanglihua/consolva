using System;
using Core;
using Domain.Logistics;
using Microsoft.AspNetCore.Mvc;

namespace shunda.Admin.Areas.Logistics.Models.LogisticsGoodsTrade;

namespace Web.Areas.Admin.Models.LogisticsGoodsTrade
{
    public class TradeGoodsSearchListQueryParam : ListQueryParam
    {
        [ModelBinder(typeof(LogisticsGoodsCateModelBinder))]
        public LogisticsGoodsCate CateQuery { get; set; }

        [ModelBinder(typeof(LogisticsGoodsStatusModelBinder))]
        public LogisticsGoodsStatus StatusQuery { get; set; }

        [ModelBinder(typeof(LogisticsGoodsTypeModelBinder))]
        public LogisticsGoodsType TypeQuery { get; set; }

        public string NameQuery { get; set; }

        public bool? HasBrandQuery { get; set; }

        public bool? HasBatteryQuery { get; set; }

        public bool? HasBambooQuery { get; set; }

        public bool? HasWoodPackageQuery { get; set; }

        public bool? IsSpecialQuery { get; set; }        

        public bool? IsForbiddenQuery { get; set; }

        public string CnHsCodeQuery { get; set; }

        public string CnHsNameQuery { get; set; }

        public string AuHsCodeQuery { get; set; }

        public string AuHsNameQuery { get; set; }
        
        public string UpdateUserQuery { get; set; }

        public DateTime? BeginUpdateDateQuery { get; set; }

        public DateTime? EndUpdateDateQuery { get; set; }
    }
}