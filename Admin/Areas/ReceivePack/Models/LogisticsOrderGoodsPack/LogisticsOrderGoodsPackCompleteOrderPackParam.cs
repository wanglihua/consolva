using System.ComponentModel.DataAnnotations;

namespace shunda.Admin.Areas.ReceivePack.Models.LogisticsOrderGoodsPack;

public class LogisticsOrderGoodsPackCompleteOrderPackParam {
    [Required(ErrorMessage = "订单不能为空！")]
    public int OrderId { get; set; }
}