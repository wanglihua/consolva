using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain;
using shunda.Domain.Logistics;
using shunda.Domain.LogisticsOrder;
using shunda.Domain.Storage;

namespace shunda.Admin.Areas.ReceivePack.Models.LogisticsOrderGoodsPack;

public class LogisticsOrderGoodsPackOrderListDialogModel {
    [BindNever]
    public WarehouseInfo Warehouse { get; set; } = new();

    [BindNever]
    public IList<LogisticsOrderStatus> OrderStatusList { get; set; } = LogisticsOrderStatus.GetList();

    [BindNever]
    public IList<Country> CountryList { get; set; } = Country.GetNoChinaList();

    [BindNever]
    public IList<LogisticsModeType> FirstModeTypeList { get; set; } = LogisticsModeType.GetList();

    [BindNever]
    public IList<LogisticsModeInfo> FirstModeList { get; set; } = new List<LogisticsModeInfo>();

    [BindNever]
    public IList<LogisticsGoodsType> GoodsTypeList { get; set; } = LogisticsGoodsType.GetCustomerList();
}