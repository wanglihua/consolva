using System.ComponentModel.DataAnnotations;

namespace shunda.Admin.Areas.ReceivePack.Models.LogisticsOrderGoodsPack;

public class LogisticsOrderGoodsPackRemovePackParam {
    public int PackId { get; set; }

    [Required(ErrorMessage = "订单不能为空！")]
    public int OrderId { get; set; }

    public IList<string> TrackingNoList { get; set; } = new List<string>();
}