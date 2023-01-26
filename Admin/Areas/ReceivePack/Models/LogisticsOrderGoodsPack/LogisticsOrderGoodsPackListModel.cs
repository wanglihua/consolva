using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Core;
using shunda.Domain.LogisticsOrder;

namespace shunda.Admin.Areas.ReceivePack.Models.LogisticsOrderGoodsPack;

public class LogisticsOrderGoodsPackListModel : ListModel {
    public LogisticsOrderGoodsPackListModel(ListPager listPager) : base(listPager) {
    }

    [BindNever]
    public IList<LogisticsOrderInfo> OrderList { get; set; } = new List<LogisticsOrderInfo>();
}