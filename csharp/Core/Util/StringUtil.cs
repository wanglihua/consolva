namespace shunda.Core.Util;

public static class StringUtil
{
    public static bool IsNumeric(string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return true;
        }

        if (int.TryParse(str, out int intVal) == false)
        {
            return false;
        }

        return true;
    }

    public static string LimitMaxLength(string str, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(str))
        {
            return String.Empty;
        }

        if (str.Length > maxLength)
        {
            str = str.Substring(0, maxLength);
        }

        return str;
    }
}