using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Core;
using shunda.Domain.Express;

namespace shunda.Admin.Areas.Express.Models.ExpressVideo;

public class ExpressVideoListQueryParam : ListQueryParam
{
    [ModelBinder(typeof(ExpressVideoTypeModelBinder))]
    public ExpressVideoType? TypeQuery { get; set; }

    public string? CustomerCodeQuery { get; set; }

    public string? OrderCodeQuery { get; set; }
    
    [BindNever]
    public int OrderId { get; set; }

    public string? TrackingNoQuery { get; set; }

    public DateTime? BeginDateQuery { get; set; }

    public DateTime? EndDateQuery { get; set; }

    public string? UserNameQuery { get; set; }
    
    [BindNever]
    public int UserId { get; set; }
}