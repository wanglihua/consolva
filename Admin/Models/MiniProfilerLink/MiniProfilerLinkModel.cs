namespace shunda.Admin.Models.MiniProfilerLink;

public class MiniProfilerLinkModel
{
#if DEBUG
    public bool Debug { get; } = true;
#else
    public bool Debug { get; } = false;
#endif
}