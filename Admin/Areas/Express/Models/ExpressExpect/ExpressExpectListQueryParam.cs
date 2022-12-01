using shunda.Core;

namespace shunda.Admin.Areas.Express.Models.ExpressExpect;

public class ExpressExpectListQueryParam : ListQueryParam
{
    public string? CustomerCodeQuery { get; set; }

    public string? TrackingNoQuery { get; set; }
}