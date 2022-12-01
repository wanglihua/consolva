using Microsoft.AspNetCore.Mvc;
using shunda.Core;
using shunda.Domain.Logistics;

namespace shunda.Admin.Areas.Logistics.Models.LogisticsGoodsWarehouse;

public class LogisticsGoodsWarehouseListQueryParam : ListQueryParam
{
    /// <summary>
    /// 这个用于添加对话框加载
    /// </summary>
    public int GoodsId { get; set; } // todo 如果不用了，就去掉

    [ModelBinder(typeof(LogisticsGoodsTypeModelBinder))]
    public LogisticsGoodsType? GoodsTypeQuery { get; set; }

    public string? NameQuery { get; set; }
}