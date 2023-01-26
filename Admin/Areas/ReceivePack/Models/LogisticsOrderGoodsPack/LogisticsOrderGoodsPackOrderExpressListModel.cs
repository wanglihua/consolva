using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Express;
using shunda.Domain.Logistics;

namespace shunda.Admin.Areas.ReceivePack.Models.LogisticsOrderGoodsPack;

public class LogisticsOrderGoodsPackOrderExpressListModel {
    [BindNever]
    public LogisticsGoodsType GoodsType { get; set; } = LogisticsGoodsType.None;

    [BindNever]
    public IList<ExpressInfo> GoodsTypeExpressList { get; set; } = new List<ExpressInfo>();

    [BindNever]
    public IDictionary<LogisticsGoodsType, bool> GoodsTypePackCompleteDict { get; set; } = new Dictionary<LogisticsGoodsType, bool>();

    /// <summary>
    /// 这个物品类型下面的总体积重
    /// 空运订单时是整个订单的总体积重
    /// </summary>
    [BindNever]
    [Obsolete]
    public decimal TotalVolumeWeight { get; set; }

    /// <summary>
    /// 这个物品类型下面的总实际重
    /// 空运订单时是整个订单的实际积重
    /// </summary>
    [BindNever]
    [Obsolete]
    public decimal TotalActualWeight { get; set; }

    [BindNever]
    public bool Debug { get; set; }
}