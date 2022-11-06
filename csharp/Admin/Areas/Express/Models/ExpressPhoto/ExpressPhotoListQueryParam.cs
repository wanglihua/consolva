using Microsoft.AspNetCore.Mvc;
using shunda.Core;
using shunda.Domain.Express;

namespace shunda.Admin.Areas.Express.Models.ExpressPhoto;

public class ExpressPhotoListQueryParam : ListQueryParam
{
    [ModelBinder(typeof(ExpressPhotoTypeModelBinder))]
    public ExpressPhotoType? TypeQuery { get; set; }

    public string? CustomerCodeQuery { get; set; }

    public string? OrderCodeQuery { get; set; }

    public string? TrackingNoQuery { get; set; }

    public DateTime? BeginDateQuery { get; set; }

    public DateTime? EndDateQuery { get; set; }

    public string? UserNameQuery { get; set; }
}