using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Logistics;
using shunda.Domain.LogisticsOrder;
using shunda.Domain.Storage;

namespace shunda.Admin.Areas.ReceivePack.Models.LogisticsOrderGoodsPack;

public class LogisticsOrderGoodsPackListDialogModel {
    [BindNever]
    public WarehouseInfo Warehouse { get; set; } = new();

    [BindNever]
    public IList<LogisticsOrderStatus> OrderStatusList { get; set; } = LogisticsOrderStatus.GetList();

    [BindNever]
    public IList<WarehouseInfo> ReceivingWarehouseList { get; set; } = new List<WarehouseInfo>();

    [Obsolete("TargetWarehouseList 不要了")]
    [BindNever]
    public IList<WarehouseInfo> TargetWarehouseList { get; set; } = new List<WarehouseInfo>();

    [BindNever]
    public IList<LogisticsModeType> FirstLogisticsModeTypeList { get; set; } = LogisticsModeType.GetList();

    [BindNever]
    public IList<LogisticsModeInfo> FirstLogisticsModeList { get; set; } = new List<LogisticsModeInfo>();

    [BindNever]
    public IList<LogisticsGoodsType> GoodsTypeList { get; set; } = LogisticsGoodsType.GetCustomerList();
}