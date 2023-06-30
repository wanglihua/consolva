using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain;
using shunda.Domain.Fee;
using shunda.Domain.Logistics;
using shunda.Domain.LogisticsOrder;
using shunda.Domain.Storage;

namespace shunda.Admin.Areas.LogisticsOrder.Models.LogisticsOrderList;

public class LogisticsOrderListIndexModel {
    [BindNever]
    public IList<Country> CountryList { get; set; } = Country.GetList();

    [BindNever]
    public IList<LogisticsOrderCate> OrderCateList { get; set; } = LogisticsOrderCate.GetList();

    [BindNever]
    public IList<LogisticsOrderStatus> OrderStatusList { get; set; } = LogisticsOrderStatus.GetList();

    [BindNever]
    public IList<PayStatus> PayStatusList { get; set; } = PayStatus.GetList();

    [BindNever]
    public IList<WarehouseInfo> ReceivingWarehouseList { get; set; } = new List<WarehouseInfo>();

    [BindNever]
    public IList<LogisticsModeType> FirstModeTypeList { get; set; } = LogisticsModeType.GetList();

    [BindNever]
    public IList<TimeRangeType> TimeRangeTypeList { get; set; } = TimeRangeType.GetList();

    [BindNever]
    public OrderListMenuType MenuType { get; set; }

    [BindNever]
    public bool CanImportOrder { get; set; }
}