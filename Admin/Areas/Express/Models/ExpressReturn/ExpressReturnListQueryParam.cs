using Microsoft.AspNetCore.Mvc;
using shunda.Core;
using shunda.Domain.Express;

namespace shunda.Admin.Areas.Express.Models.ExpressReturn;

public class ExpressReturnListQueryParam : ListQueryParam {
    public int WarehouseIdQuery { get; set; }

    public string? CheckinTrackingNoQuery { get; set; }

    public string? TrackingNoQuery { get; set; }

    public string? CustomerCodeQuery { get; set; }

    public string? ExpressCompanyQuery { get; set; }

    public string? ReturnTrackingNoQuery { get; set; }

    public string? ReturnExpressCompanyQuery { get; set; }

    [ModelBinder(typeof(ExpressStatusModelBinder))]
    public ExpressStatus? StatusQuery { get; set; }

    public DateTime? AddTimeBeginQuery { get; set; }

    public DateTime? AddTimeEndQuery { get; set; }

    public DateTime? ReturnApplyTimeBeginQuery { get; set; }

    public DateTime? ReturnApplyTimeEndQuery { get; set; }

    public DateTime? ReturnTimeBeginQuery { get; set; }

    public DateTime? ReturnTimeEndQuery { get; set; }
}