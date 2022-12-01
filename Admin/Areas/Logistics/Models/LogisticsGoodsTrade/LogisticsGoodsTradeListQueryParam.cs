using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Core;
using shunda.Domain.Logistics;

namespace shunda.Admin.Areas.Logistics.Models.LogisticsGoodsTrade;

public class TradeGoodsSearchListQueryParam : ListQueryParam
{
    [ModelBinder(typeof(LogisticsGoodsStatusModelBinder))]
    public LogisticsGoodsStatus? StatusQuery { get; set; }

    [ModelBinder(typeof(LogisticsGoodsTypeModelBinder))]
    public LogisticsGoodsType? TypeQuery { get; set; }

    public string? NameQuery { get; set; }

    public string? CnHsCodeQuery { get; set; }

    public string? CnHsNameQuery { get; set; }

    public string? EnHsCodeQuery { get; set; }

    public string? EnHsNameQuery { get; set; }

    public string? UpdateUserQuery { get; set; }

    [BindNever]
    public int UpdateUserId { get; set; } = 0;

    public DateTime? BeginUpdateDateQuery { get; set; }

    public DateTime? EndUpdateDateQuery { get; set; }
}