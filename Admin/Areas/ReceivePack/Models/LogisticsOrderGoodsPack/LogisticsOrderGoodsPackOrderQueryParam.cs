namespace shunda.Admin.Areas.ReceivePack.Models.LogisticsOrderGoodsPack;

public class LogisticsOrderGoodsPackOrderQueryParam {
    /// <summary>
    /// 订单Id
    /// </summary>
    public int? OrderId { get; set; }

    /// <summary>
    /// 下一个打包订单Id
    /// </summary>
    public int? NextOrderId { get; set; }

    /// <summary>
    /// 仓库Id
    /// </summary>
    public int WarehouseId { get; set; }
}