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
    
    [BindNever]
    public decimal TotalVolumeWeight { get; set; }

    [BindNever]
    public decimal TotalActualWeight { get; set; }    

    [BindNever]
    public bool Debug { get; set; }
}