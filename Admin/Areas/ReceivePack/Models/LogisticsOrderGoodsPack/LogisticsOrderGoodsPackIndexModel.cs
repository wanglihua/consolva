using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Storage;

namespace shunda.Admin.Areas.ReceivePack.Models.LogisticsOrderGoodsPack;

public class LogisticsOrderGoodsPackIndexModel {
    [BindNever]
    public WarehouseInfo Warehouse { get; set; } = new();

#if DEBUG
    public bool Debug { get; } = true;
#else
        public bool Debug { get; } = false;
#endif
}