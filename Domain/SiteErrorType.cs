using shunda.Core.CodeType;

namespace shunda.Domain;

/// <summary>
/// 网站错误类型
/// </summary>
public class SiteErrorType : CodeTypeBase
{
    /// <summary>
    /// 短信发送
    /// </summary>
    public static readonly SiteErrorType SmsSend = new SiteErrorType(1000, "短信发送", "短信发送");

    /// <summary>
    /// 运费预估
    /// </summary>
    public static readonly SiteErrorType FreightEstimate = new SiteErrorType(2000, "运费预估", "运费预估");

    /// <summary>
    /// 汇率更新
    /// </summary>
    public static readonly SiteErrorType ExchangeRate = new SiteErrorType(3000, "汇率更新", "汇率更新");

    /// <summary>
    /// None
    /// </summary>
    public static readonly SiteErrorType None = new SiteErrorType(0, "", "");

    private static readonly IReadOnlyList<SiteErrorType> codeTypeList = new List<SiteErrorType>() {
        SmsSend,
        FreightEstimate,
        ExchangeRate,
    };

    private static Dictionary<int, SiteErrorType> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static SiteErrorType GetById(int id)
    {
        if (codeTypeMap.TryGetValue(id, out SiteErrorType? codeType))
        {
            return codeType;
        }

        return None;
    }

    public static IList<SiteErrorType> GetList()
    {
        return codeTypeList.ToList();
    }

    private SiteErrorType(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class SiteErrorTypeHibernateType : CodeHibernateTypeBase<SiteErrorType>
{
    protected override SiteErrorType GetCodeTypeById(int id)
    {
        return SiteErrorType.GetById(id);
    }
}

public class SiteErrorTypeModelBinder : CodeTypeModelBinderBase<SiteErrorType>
{
    protected override SiteErrorType GetCodeTypeById(int id)
    {
        return SiteErrorType.GetById(id);
    }
}