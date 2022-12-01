using Newtonsoft.Json;

namespace shunda.Core.CodeType;

public abstract class CodeTypeBase
{
    /// <summary>
    /// 编码
    /// </summary>
    [JsonProperty("id")]
    public int Id { get; protected set; }

    /// <summary>
    /// 名称
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; protected set; }

    /// <summary>
    /// 描述
    /// </summary>
    [JsonProperty("description")]
    public string Description { get; protected set; }

    public CodeTypeBase(int id, string name, string description)
    {
        Id = id;
        Name = name;
        Description = description;
    }

    /// <summary>
    /// 字符串表示, 这个的实现方式很重要
    /// </summary>
    /// <returns>string</returns>
    public override string ToString()
    {
        return Id.ToString();
    }

    /// <summary>
    /// 判断相等, 这个的实现方式很重要
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public new bool Equals(object x, object y)
    {
        if (x == null && y == null) // 两个同时为 null
        {
            return true;
        }

        if (x == null || y == null) // 两个中只有其中一个为 null
        {
            return false;
        }

        if (x is CodeTypeBase == false)
        {
            return false;
        }

        if (y is CodeTypeBase == false)
        {
            return false;
        }

        CodeTypeBase? xCodeType = x as CodeTypeBase;
        CodeTypeBase? yCodeType = y as CodeTypeBase;

        return xCodeType?.Id == yCodeType?.Id;
    }
}