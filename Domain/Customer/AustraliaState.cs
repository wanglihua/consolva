using shunda.Core.CodeType;

namespace shunda.Domain.Customer;

/// <summary>
/// 澳大利亚州信息
/// </summary>
public class AustraliaState : StringCodeTypeBase {
    public static readonly AustraliaState QLD = new AustraliaState("QLD", "QLD", "QLD");
    public static readonly AustraliaState VIC = new AustraliaState("VIC", "VIC", "VIC");
    public static readonly AustraliaState NSW = new AustraliaState("NSW", "NSW", "NSW");
    public static readonly AustraliaState ACT = new AustraliaState("ACT", "ACT", "ACT");
    public static readonly AustraliaState SA = new AustraliaState("SA", "SA", "SA");
    public static readonly AustraliaState WA = new AustraliaState("WA", "WA", "WA");
    public static readonly AustraliaState TAS = new AustraliaState("TAS", "TAS", "TAS");
    public static readonly AustraliaState NT = new AustraliaState("NT", "NT", "NT");

    public static readonly AustraliaState None = new AustraliaState("", "", "");

    private static readonly IReadOnlyList<AustraliaState> codeTypeList = new List<AustraliaState>() {
        QLD,
        VIC,
        NSW,
        ACT,
        SA,
        WA,
        TAS,
        NT,
    };

    private static readonly IDictionary<string, AustraliaState> codeTypeMap = codeTypeList.ToDictionary(e => e.Code);

    public static AustraliaState GetByCode(string code) {
        if (codeTypeMap.TryGetValue(code, out AustraliaState? codeType)) {
            return codeType;
        }

        return None;
    }

    public static IList<AustraliaState> GetList() {
        return codeTypeList.ToList();
    }

    private AustraliaState(string code, string name, string description) : base(code, name, description) {
    }
}

public class AustraliaStateHibernateType : StringCodeHibernateTypeBase<AustraliaState> {
    protected override AustraliaState GetCodeTypeByCode(string code) {
        return AustraliaState.GetByCode(code);
    }
}

public class AustraliaStateModelBinder : StringCodeTypeModelBinderBase<AustraliaState> {
    protected override AustraliaState GetCodeTypeByCode(string code) {
        return AustraliaState.GetByCode(code);
    }
}