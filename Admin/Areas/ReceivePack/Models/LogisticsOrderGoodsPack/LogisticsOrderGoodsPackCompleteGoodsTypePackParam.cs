using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using shunda.Domain.Logistics;

namespace shunda.Admin.Areas.ReceivePack.Models.LogisticsOrderGoodsPack;

public class LogisticsOrderGoodsPackCompleteGoodsTypePackParam {
    [Required(ErrorMessage = "订单不能为空！")]
    public int OrderId { get; set; }

    [Required(ErrorMessage = "物品类型不能为空！")]
    [ModelBinder(typeof(LogisticsGoodsTypeModelBinder))]
    public LogisticsGoodsType GoodsType { get; set; } = LogisticsGoodsType.None;

    public IList<string> ExpressTrackingNoList { get; set; } = new List<string>();
}