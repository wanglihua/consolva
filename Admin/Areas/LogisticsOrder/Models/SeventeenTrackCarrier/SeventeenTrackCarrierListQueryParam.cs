using shunda.Core;
using shunda.Domain.LogisticsOrder;
using Microsoft.AspNetCore.Mvc;

namespace shunda.Admin.Areas.LogisticsOrder.Models.SeventeenTrackCarrier;

public class SeventeenTrackCarrierListQueryParam : ListQueryParam {
    public string? CarrierKeyQuery { get; set; }

    public string? CarrierNameQuery { get; set; }
}