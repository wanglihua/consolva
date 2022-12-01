using System.Diagnostics.CodeAnalysis;

namespace shunda.Core;

public class ListModel
{
    public ListModel([NotNull] ListPager listPager)
    {
        this.listPager = listPager;
    }

    private ListPager listPager;

    public ListPager ListPager
    {
        get { return listPager; }
    }

    public int ItemBegin
    {
        get { return listPager.ItemBegin; }
    }
}