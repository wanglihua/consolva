using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace shunda.Admin.Areas.Logistics.Models.LogisticsGoodsWarehouse;

public class LogisticsGoodsWarehouseImageModel
{
    [BindNever]
    public string ImageFileUrl { get; set; } = string.Empty;

    public string StorePath { get; set; } = string.Empty;

    public string StoreName { get; set; } = string.Empty;
}