using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.LogisticsOrder;
using shunda.Domain.Storage;

namespace shunda.Admin.Areas.ReceivePack.Models.LogisticsOrderGoodsPack;

public class LogisticsOrderGoodsPackIndexModel {
    [BindNever]
    public WarehouseInfo Warehouse { get; set; } = new();

    [BindNever]
    public IList<LogisticsOrderPackPrintType> PrintTypeList { get; set; } = LogisticsOrderPackPrintType.GetList();

    [BindNever]
    public LogisticsOrderPackPrintType PrintTypeSelected { get; set; } = LogisticsOrderPackPrintType.None;


#if DEBUG
    public bool Debug { get; } = true;
#else
        public bool Debug { get; } = false;
#endif
}