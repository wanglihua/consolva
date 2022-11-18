using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace shunda.Admin.Areas.Logistics.Models.LogisticsGoods;

public class LogisticsGoodsImageModel
{
    [BindNever]
    public string ImageFileUrl { get; set; } = string.Empty;

    public string StorePath { get; set; } = string.Empty;

    public string StoreName { get; set; } = string.Empty;
}