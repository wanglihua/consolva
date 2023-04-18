using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Core;
using shunda.Domain.Logistics;

namespace shunda.Admin.Areas.LogisticsOrder.Models.LogisticsOrderFirstMode;

public class LogisticsOrderFirstModeListQueryParam : ListQueryParam {
    public string? OrderCodeQuery { get; set; }

    [BindNever]
    public int? OrderIdQuery { get; set; }

    public string? FirstModeNameQuery { get; set; }

    [ModelBinder(typeof(LogisticsGoodsTypeModelBinder))]
    public LogisticsGoodsType? GoodsTypeQuery { get; set; }

    public string? TrackingNoQuery { get; set; }

    public DateTime? BeginDateQuery { get; set; }

    public DateTime? EndDateQuery { get; set; }
}