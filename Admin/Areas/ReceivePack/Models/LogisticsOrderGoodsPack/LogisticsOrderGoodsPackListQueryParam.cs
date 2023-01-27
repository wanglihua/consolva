using Microsoft.AspNetCore.Mvc;
using shunda.Core;
using shunda.Domain;
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

    [ModelBinder(typeof(CountryModelBinder))]
    public Country? CountryQuery { get; set; }

    [Obsolete]
    [ModelBinder(typeof(LogisticsModeTypeModelBinder))]
    public LogisticsModeType? FirstModeTypeQuery { get; set; }

    public int FirstModeIdQuery { get; set; }

    [ModelBinder(typeof(LogisticsGoodsTypeModelBinder))]
    public LogisticsGoodsType? GoodsTypeQuery { get; set; }

    public DateTime? BeginDateQuery { get; set; }

    public DateTime? EndDateQuery { get; set; }
}