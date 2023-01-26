using Microsoft.AspNetCore.Mvc;
using shunda.Core;
using shunda.Domain.Logistics;
using shunda.Domain.LogisticsOrder;

namespace shunda.Admin.Areas.ReceivePack.Models.LogisticsOrderGoodsPack;

public class LogisticsOrderGoodsPackListQueryParam : ListQueryParam {
    public override int PageSize {
        get { return 10; }
    }

    /// <summary>
    /// 仓库 Id
    /// </summary>
    public int WarehouseId { get; set; }

    /// <summary>
    /// 订单编号查询
    /// </summary>
    public string? OrderCodeQuery { get; set; }

    [ModelBinder(typeof(LogisticsOrderStatusModelBinder))]
    public LogisticsOrderStatus? OrderStatusQuery { get; set; }

    [Obsolete]
    // 这个不需要查询的，收货仓库已经是确定了的
    public int ReceivingWarehouseIdQuery { get; set; }

    [Obsolete]
    public int TargetWarehouseIdQuery { get; set; }

    // 这个也不需要查询的，头程物流方式也已经是固定了的
    [ModelBinder(typeof(LogisticsModeTypeModelBinder))]
    public LogisticsModeType? FirstLogisticsModeTypeQuery { get; set; }

    public int FirstLogisticsModeIdQuery { get; set; }

    [ModelBinder(typeof(LogisticsGoodsTypeModelBinder))]
    public LogisticsGoodsType? GoodsTypeQuery { get; set; }

    public DateTime? BeginDateQuery { get; set; }

    public DateTime? EndDateQuery { get; set; }
}