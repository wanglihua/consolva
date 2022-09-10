namespace shunda.Core;

public class ListQueryParam
{
    public virtual int PageSize
    {
        get { return 15; }
    }

    public int PageIndex { get; set; } = 0;

    public string SortField { get; set; } = nameof(Entity.Id);

    public string SortDir { get; set; } = SortDirection.Desc;
}