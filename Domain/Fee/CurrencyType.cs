using shunda.Core.CodeType;

namespace shunda.Domain.Fee;

/// <summary>
/// 货币类型
/// </summary>
public class CurrencyType : StringCodeTypeBase {
    /// <summary>
    /// 人民币
    /// </summary>
    public static readonly CurrencyType CNY = new CurrencyType("CNY", "人民币", "人民币");

    /// <summary>
    /// 澳币
    /// </summary>
    public static readonly CurrencyType AUD = new CurrencyType("AUD", "澳币", "澳币");

    /// <summary>
    /// 新西兰元
    /// </summary>
    public static readonly CurrencyType NZD = new CurrencyType("NZD", "新西兰元", "新西兰元");

    /// <summary>
    /// 美元
    /// </summary>
    public static readonly CurrencyType USD = new CurrencyType("USD", "美元", "美元");

    /// <summary>
    /// 加拿大元
    /// </summary>
    public static readonly CurrencyType CAD = new CurrencyType("CAD", "加拿大元", "加拿大元");

    /// <summary>
    /// 欧元
    /// </summary>
    public static readonly CurrencyType EUR = new CurrencyType("EUR", "欧元", "欧元");

    /// <summary>
    /// 港币
    /// </summary>
    public static readonly CurrencyType HKD = new CurrencyType("HKD", "港币", "港币");

    /// <summary>
    /// 日元
    /// </summary>
    public static readonly CurrencyType JPY = new CurrencyType("JPY", "日元", "日元");

    /// <summary>
    /// 韩元
    /// </summary>
    public static readonly CurrencyType KRW = new CurrencyType("KRW", "韩元", "韩元");

    /// <summary>
    /// 新加坡元
    /// </summary>
    public static readonly CurrencyType SGD = new CurrencyType("SGD", "新加坡元", "新加坡元");

    /// <summary>
    /// 林吉特
    /// </summary>
    public static readonly CurrencyType MYR = new CurrencyType("MYR", "林吉特", "马来西亚林吉特");

    // MYR

    /// <summary>
    /// None
    /// </summary>
    public static readonly CurrencyType None = new CurrencyType("", "", "");

    private static readonly IReadOnlyList<CurrencyType> codeTypeList = new List<CurrencyType>() {
        CNY,
        AUD,
        NZD,
        USD,
        CAD,
        EUR,
        HKD,
        JPY,
        KRW,
        SGD,
        MYR,
    };

    private static readonly Dictionary<string, CurrencyType> codeTypeMap = codeTypeList.ToDictionary(e => e.Code);


    public static IList<CurrencyType> GetList() {
        return codeTypeList.ToList();
    }

    public static CurrencyType GetByCode(string code) {
        if (string.IsNullOrWhiteSpace(code)) {
            return None;
        }

        code = code.ToUpper();
        if (codeTypeMap.TryGetValue(code, out CurrencyType? codeType)) {
            return codeType;
        }

        return None;
    }

    private CurrencyType(string code, string name, string description) : base(code, name, description) {
    }
}

public class CurrencyTypeHibernateType : StringCodeHibernateTypeBase<CurrencyType> {
    protected override CurrencyType GetCodeTypeByCode(string code) {
        return CurrencyType.GetByCode(code);
    }
}

public class CurrencyTypeModelBinder : StringCodeTypeModelBinderBase<CurrencyType> {
    protected override CurrencyType GetCodeTypeByCode(string code) {
        return CurrencyType.GetByCode(code);
    }
}