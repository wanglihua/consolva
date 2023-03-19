using Newtonsoft.Json;

namespace shunda.Core.CodeType;

public abstract class StringCodeTypeBase : IComparable {
    /// <summary>
    /// 编码
    /// </summary>
    [JsonProperty("code")]
    public string Code { get; protected set; } = string.Empty;

    /// <summary>
    /// 名称
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; protected set; } = string.Empty;

    /// <summary>
    /// 描述
    /// </summary>
    [JsonProperty("description")]
    public string Description { get; protected set; } = string.Empty;

    protected StringCodeTypeBase(string code, string name, string description) {
        Code = code;
        Name = name;
        Description = description;
    }

    /// <summary>
    /// 字符串表示, 这个的实现方式很重要
    /// </summary>
    /// <returns>string</returns>
    public override string ToString() {
        return Code;
    }

    public virtual int CompareTo(object? obj) {
        if (obj == null) {
            return 1;
        }

        var codeType = obj as StringCodeTypeBase;
        if (codeType == null) {
            return 1;
        }

        return String.Compare(this.Code, codeType.Code, StringComparison.Ordinal);
    }

    /// <summary>
    /// 判断相等, 这个的实现方式很重要
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public new bool Equals(object x, object y) {
        if (x is StringCodeTypeBase == false) {
            return false;
        }

        if (y is StringCodeTypeBase == false) {
            return false;
        }

        StringCodeTypeBase? xCodeType = x as StringCodeTypeBase;
        StringCodeTypeBase? yCodeType = y as StringCodeTypeBase;

        return xCodeType?.Code == yCodeType?.Code;
    }
}