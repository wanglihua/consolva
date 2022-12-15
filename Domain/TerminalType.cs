using shunda.Core.CodeType;

namespace shunda.Domain;

/// <summary>
/// 应用终端类型
/// </summary>
public class TerminalType : CodeTypeBase
{
    /// <summary>
    /// 微信小程序
    /// </summary>
    public static readonly TerminalType WechatApp = new TerminalType(1000, "微信小程序", "微信小程序");

    /// <summary>
    /// None
    /// </summary>
    public static readonly TerminalType None = new TerminalType(0, "", "");

    private static readonly IReadOnlyList<TerminalType> codeTypeList = new List<TerminalType>() {
        WechatApp,
    };

    private static Dictionary<int, TerminalType> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static TerminalType GetById(int id)
    {
        if (codeTypeMap.TryGetValue(id, out TerminalType? codeType))
        {
            return codeType;
        }

        return None;
    }

    public static IList<TerminalType> GetList()
    {
        return codeTypeList.ToList();
    }

    private TerminalType(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class TerminalTypeHibernateType : CodeHibernateTypeBase<TerminalType>
{
    protected override TerminalType GetCodeTypeById(int id)
    {
        return TerminalType.GetById(id);
    }
}

public class TerminalTypeModelBinder : CodeTypeModelBinderBase<TerminalType>
{
    protected override TerminalType GetCodeTypeById(int id)
    {
        return TerminalType.GetById(id);
    }
}