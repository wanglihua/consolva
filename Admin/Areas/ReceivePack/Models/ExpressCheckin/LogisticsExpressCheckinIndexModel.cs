using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Express;
using shunda.Domain.Storage;

namespace shunda.Admin.Areas.ReceivePack.Models.ExpressCheckin;

public class LogisticsExpressCheckinIndexModel
{
    [BindNever]
    public WarehouseInfo Warehouse { get; set; } = new ();

    [BindNever]
    public ExpressCheckinLabelType CheckinPrintType { get; set; } = ExpressCheckinLabelType.None; // todo a 改名

    [BindNever]
    public int CheckinPrintCount { get; set; }

    [BindNever]
    public string CheckinLoginName { get; set; } = string.Empty;

    [BindNever]
    public string CheckinUserName { get; set; } = string.Empty;

#if DEBUG
    public bool Debug { get; } = true;
#else
        public bool Debug { get; } = false;
#endif
}