using Microsoft.AspNetCore.Mvc;
using shunda.Core;

namespace shunda.Domain;

public class SiteErrorInfo : Entity
{
    [ModelBinder(typeof(SiteErrorTypeModelBinder))]
    public virtual SiteErrorType Type { get; set; } = SiteErrorType.None;

    public virtual string SubType { get; set; } = string.Empty;

    public virtual string ErrorMsg { get; set; } = string.Empty;

    public virtual bool Resolved { get; set; }
}