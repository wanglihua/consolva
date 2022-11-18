using Microsoft.AspNetCore.Mvc;
using shunda.Core;
using shunda.Domain.Logistics;

namespace shunda.Admin.Areas.Logistics.Models.LogisticsGoods;

public class LogisticsGoodsListQueryParam : ListQueryParam
{
    [ModelBinder(typeof(LogisticsGoodsTypeModelBinder))]
    public LogisticsGoodsType? TypeQuery { get; set; }

    public string? NameQuery { get; set; }

    public string? CnHsCodeQuery { get; set; }

    public string? CnHsNameQuery { get; set; }

    public string? EnHsCodeQuery { get; set; }

    public string? EnHsNameQuery { get; set; }
}