namespace shunda.Core.CodeType;

public abstract class StringCodeTypeBase
{
    /// <summary>
    /// 编码
    /// </summary>
    public string Code { get; protected set; } = string.Empty;

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; protected set; } = string.Empty;

    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; protected set; } = string.Empty;

    /// <summary>
    /// 字符串表示, 这个的实现方式很重要
    /// </summary>
    /// <returns>string</returns>
    public override string ToString()
    {
        return Code;
    }

    /// <summary>
    /// 判断相等, 这个的实现方式很重要
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public new bool Equals(object x, object y)
    {
        if (x is StringCodeTypeBase == false)
        {
            return false;
        }

        if (y is StringCodeTypeBase == false)
        {
            return false;
        }

        StringCodeTypeBase? xCodeType = x as StringCodeTypeBase;
        StringCodeTypeBase? yCodeType = y as StringCodeTypeBase;

        return xCodeType?.Code == yCodeType?.Code;
    }
}