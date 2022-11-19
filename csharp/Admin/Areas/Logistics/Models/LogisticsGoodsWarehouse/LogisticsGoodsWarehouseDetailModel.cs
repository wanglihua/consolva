using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Logistics;

namespace shunda.Admin.Areas.Logistics.Models.LogisticsGoodsWarehouse;

public class LogisticsGoodsWarehouseDetailModel : LogisticsGoodsWarehouseSaveModel
{
    [BindNever]
    public IList<LogisticsGoodsType> TypeList { get; set; } = LogisticsGoodsType.GetInnerList();

    [BindNever]
    public IList<LogisticsGoodsImageInfo> ImageList { get; set; } = new List<LogisticsGoodsImageInfo>();
}