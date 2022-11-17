using System;
using Core;
using Domain.Logistics;
using Microsoft.AspNetCore.Mvc;

namespace shunda.Admin.Areas.Logistics.Models.LogisticsGoodsWarehouse;

namespace Web.Areas.Admin.Models.LogisticsGoodsWarehouse
{
    public class LogisticsGoodsWarehouseListQueryParam : ListQueryParam
    {
        /// <summary>
        /// 这个用于添加对话框加载
        /// </summary>
        public int GoodsId { get; set; }

        [ModelBinder(typeof(LogisticsGoodsCateModelBinder))]
        public LogisticsGoodsCate CateQuery { get; set; }

        [ModelBinder(typeof(LogisticsGoodsTypeModelBinder))]
        public LogisticsGoodsType TypeQuery { get; set; }

        public string NameQuery { get; set; }

        public bool? HasBrandQuery { get; set; }

        public bool? HasBatteryQuery { get; set; }

        public bool? HasBambooQuery { get; set; }

        public bool? HasWoodPackageQuery { get; set; }

        public bool? IsSpecialQuery { get; set; }        

        public bool? IsForbiddenQuery { get; set; }
    }
}
