using shunda.Core;

namespace shunda.Domain;

public class SiteRequestLogInfo : EntityNoCreateNoUpdate
{
    public virtual string SessionId { get; set; } = string.Empty;

    public virtual string IpAddress { get; set; } = string.Empty;

    public virtual int? UserId { get; set; }

    public virtual int? CustomerId { get; set; }

    public virtual string Url { get; set; } = string.Empty;

    public virtual string? FormParams { get; set; }

    public virtual decimal TotalSeconds { get; set; } = 0m;

    public virtual DateTime CreateTime { get; set; } = DateTime.Now;
}