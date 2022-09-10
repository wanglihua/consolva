namespace shunda.Core;

public class SortDirection
{
    public static readonly string Asc = "asc";
    public static readonly string Desc = "desc";

    public static bool IsAscending(string sortDir)
    {
        if (string.IsNullOrWhiteSpace(sortDir))
        {
            return true;
        }

        if (sortDir.ToLower() == Desc.ToLower())
        {
            return false;
        }

        return true;
    }
}