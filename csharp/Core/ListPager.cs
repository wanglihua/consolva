namespace shunda.Core;

public class ListPager
{
    public ListPager()
    {
    }

    public ListPager(ListQueryParam queryParam, int totalCount)
    {
        PageIndex = queryParam.PageIndex;
        PageSize = queryParam.PageSize;
        SortField = queryParam.SortField;
        SortDir = queryParam.SortDir;
        TotalCount = totalCount;
    }

    #region 通过设置取到值的

    private int pageIndex = 0;

    public int PageIndex
    {
        get
        {
            if (pageIndex >= PageCount)
            {
                pageIndex = PageCount - 1;
            }

            return pageIndex;
        }
        set { pageIndex = value; }
    }

    public int PageSize { get; set; }

    public int TotalCount { get; set; }

    public string SortField { get; set; } = string.Empty;

    public string SortDir { get; set; } = String.Empty;

    #endregion

    #region 通过计算得到值的

    public bool IsAscending
    {
        get { return SortDirection.IsAscending(SortDir); }
    }

    public int PageCount
    {
        get
        {
            if (TotalCount <= 0)
            {
                return 1;
            }

            if (PageSize <= 0)
            {
                return 1;
            }

            return (TotalCount + PageSize - 1) / PageSize;
        }
    }

    public int ItemBegin
    {
        get
        {
            if (TotalCount == 0)
            {
                return 0;
            }

            return PageIndex * PageSize + 1;
        }
    }

    public int ItemEnd
    {
        get
        {
            int itemEndByPageSize = (PageIndex + 1) * PageSize;
            if (itemEndByPageSize > TotalCount)
            {
                // 这里已经把 TotalCount = 0 的情况也考虑进去了
                return TotalCount;
            }

            return itemEndByPageSize;
        }
    }

    public int FirstPageIndex
    {
        get { return 0; }
    }

    public int PrePageIndex
    {
        get
        {
            int prePageIndex = PageIndex - 1;
            if (prePageIndex < 0)
            {
                return 0;
            }

            return prePageIndex;
        }
    }

    public int NextPageIndex
    {
        get
        {
            int nextPageIndex = PageIndex + 1;
            if (nextPageIndex > PageCount - 1)
            {
                return PageCount - 1;
            }

            return nextPageIndex;
        }
    }

    public int LastPageIndex
    {
        get { return (PageCount - 1); }
    }

    public IList<int> PageList
    {
        get
        {
            IList<int> pageList = new List<int>();

            for (int index = 0; index < PageCount; index++)
            {
                pageList.Add(index);
            }

            return pageList;
        }
    }

    public int Skip()
    {
        return PageIndex * PageSize;
    }

    public int Take()
    {
        return PageSize;
    }

    #endregion
}