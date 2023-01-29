using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Logistics;
using shunda.Domain.LogisticsOrder;

namespace shunda.Admin.Areas.ReceivePack.Models.LogisticsOrderGoodsPack;

public class LogisticsOrderGoodsPackOrderPackListModel {
    [BindNever]
    public LogisticsGoodsType GoodsType { get; set; } = LogisticsGoodsType.None;

    [BindNever]
    public bool GoodsTypePackComplete { get; set; }

    [BindNever]
    public IList<LogisticsOrderPackInfo> OrderPackList { get; set; } = new List<LogisticsOrderPackInfo>();

    [BindNever]
    public bool Repack { get; set; }

    [BindNever]
    public IList<LogisticsGoodsType> GoodsTypeList { get; set; } = LogisticsGoodsType.GetCustomerList();

    [BindNever]
    public IList<LogisticsModeInfo> FirstModeList { get; set; } = new List<LogisticsModeInfo>();
}