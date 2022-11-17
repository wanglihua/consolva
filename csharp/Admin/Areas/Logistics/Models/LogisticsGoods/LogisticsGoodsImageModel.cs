using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace shunda.Admin.Areas.Logistics.Models.LogisticsGoods;

public class LogisticsGoodsImageModel
{
    [BindNever]
    public string ImageFileUrl { get; set; } = string.Empty;

    public string SubPath { get; set; } = string.Empty;

    public string FileName { get; set; } = string.Empty;
}