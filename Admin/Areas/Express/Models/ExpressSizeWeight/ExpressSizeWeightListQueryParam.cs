using shunda.Core;

namespace shunda.Admin.Areas.Express.Models.ExpressSizeWeight;

public class ExpressSizeWeightListQueryParam : ListQueryParam
{
    public string? TrackingNoQuery { get; set; }

    public DateTime? CreateDateBeginQuery { get; set; }

    public DateTime? CreateDateEndQuery { get; set; }
}