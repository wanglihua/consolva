using Microsoft.AspNetCore.Html;

namespace shunda.Core.Web;

public static class ListDataUtil
{
    #region string

    public static HtmlString Render(string? data)
    {
        if (string.IsNullOrWhiteSpace(data))
        {
            return new HtmlString("<span>-</span>");
        }


        return new HtmlString($"<span>{data}</span>");
    }

    public static HtmlString RenderStringWithDiv(string? data)
    {
        if (string.IsNullOrWhiteSpace(data))
        {
            return new HtmlString("<div><span>-</span></div>");
        }


        return new HtmlString($"<div><span>{data}</span></div>");
    }

    public static HtmlString RenderStringWithWidth(string? data, int? width = null, bool center = false)
    {
        if (string.IsNullOrWhiteSpace(data))
        {
            return new HtmlString("<div><span>-</span></div>");
        }

        if (width.HasValue)
        {
            if (center == false)
            {
                return new HtmlString($"<div class=\"break-word\" style=\"width:{width.Value}px\"><span>{data}</span></div>");
            }
            else
            {
                // 好像这里的文本居中，不起作用
                return new HtmlString($"<div class=\"break-word\" style=\"width:{width.Value}px;text-align:center;\"><span>{data}</span></div>");
            }
        }
        else
        {
            return new HtmlString($"<div><span>{data}</span></div>");
        }
    }

    #endregion

    #region Date DateTime

    public static HtmlString Render(DateTime? data, string format = "yyyy-MM-dd HH:mm")
    {
        if (data == null)
        {
            return new HtmlString("<span>-</span>");
        }

        return Render(data.Value, format);
    }

    public static HtmlString Render(DateTime data, string format = "yyyy-MM-dd HH:mm")
    {
        return new HtmlString($"<span>{data.ToString(format)}</span>");
    }

    public static HtmlString RenderDateTimeWithDiv(DateTime? data, string format = "yyyy-MM-dd HH:mm")
    {
        if (data == null)
        {
            return new HtmlString("<div><span>-</span></div>");
        }

        return new HtmlString($"<div><span>{data.Value.ToString(format)}</span></div>");
    }

    public static HtmlString RenderDate(DateTime? data, string format = "yyyy-MM-dd")
    {
        if (data == null)
        {
            return new HtmlString("<span>-</span>");
        }

        return RenderDate(data.Value);
    }

    public static HtmlString RenderDate(DateTime data, string format = "yyyy-MM-dd")
    {
        return new HtmlString($"<span>{data.ToString(format)}</span>");
    }

    public static HtmlString RenderDateWithDiv(DateTime? data, string format = "yyyy-MM-dd")
    {
        if (data == null)
        {
            return new HtmlString("<div><span>-</span></div>");
        }

        return new HtmlString($"<div><span>{data.Value.ToString(format)}</span></div>");
    }

    #endregion

    #region int

    public static HtmlString Render(int? data)
    {
        if (data == null)
        {
            return new HtmlString("<span>-</span>");
        }

        return Render(data.Value);
    }

    public static HtmlString Render(int data)
    {
        return new HtmlString($"<span>{data}</span>");
    }

    public static HtmlString RenderIntWithDiv(int? data)
    {
        if (data == null)
        {
            return new HtmlString("<div><span>-</span></div>");
        }

        return new HtmlString($"<div><span>{data}</span></div>");
    }

    #endregion

    #region decimal

    public static HtmlString Render(decimal? data, string format = "#0.00")
    {
        if (data == null)
        {
            return new HtmlString("<span>-</span>");
        }

        return Render(data.Value, format);
    }

    public static HtmlString Render(decimal data, string format = "#0.00")
    {
        return new HtmlString($"<span>{data.ToString(format)}</span>");
    }

    public static HtmlString RenderDecimalWithDiv(decimal? data, string format = "#0.00")
    {
        if (data == null)
        {
            return new HtmlString("<div><span>-</span></div>");
        }

        return new HtmlString($"<div><span>{data.Value.ToString(format)}</span></div>");
    }

    #endregion

    #region bool

    public static HtmlString Render(bool? data, string trueStr = "是", string falseStr = "否")
    {
        if (data == null)
        {
            return new HtmlString("<span>-</span>");
        }

        return Render(data.Value, trueStr, falseStr);
    }

    public static HtmlString Render(bool data, string trueStr = "是", string falseStr = "否")
    {
        if (data)
        {
            return new HtmlString($"<span>{trueStr}</span>");
        }

        return new HtmlString($"<span>{falseStr}</span>");
    }

    public static HtmlString RenderBoolWithDiv(bool data, string trueStr = "是", string falseStr = "否")
    {
        if (data)
        {
            return new HtmlString($"<div><span>{trueStr}</span></div>");
        }

        return new HtmlString($"<div><span>{falseStr}</span></div>");
    }

    public static HtmlString RenderBoolWithDiv(bool? data, string trueStr = "是", string falseStr = "否")
    {
        if (data == null)
        {
            return new HtmlString("<div><span>-</span></div>");
        }

        return RenderBoolWithDiv(data.Value, trueStr, falseStr);
    }

    #endregion
}