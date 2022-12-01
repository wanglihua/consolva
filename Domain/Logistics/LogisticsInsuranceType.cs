using shunda.Core.CodeType;

namespace shunda.Domain.Logistics;

/// <summary>
/// 保险费用计算方式
/// </summary>
public class LogisticsInsuranceType : CodeTypeBase
{
    /// <summary>
    /// 固定金额，1000
    /// </summary>
    public static readonly LogisticsInsuranceType FixedFee = new LogisticsInsuranceType(1000, "固定金额", "固定金额");

    /// <summary>
    ///  订单金额比例, 2000
    /// </summary>
    public static readonly LogisticsInsuranceType PercentFee = new LogisticsInsuranceType(2000, "订单金额比例", "订单金额比例");

    /// <summary>
    /// None
    /// </summary>
    public static readonly LogisticsInsuranceType None = new LogisticsInsuranceType(0, "", "");

    private static readonly IReadOnlyList<LogisticsInsuranceType> codeTypeList = new List<LogisticsInsuranceType>() {
        FixedFee,
        PercentFee,
    };

    private static readonly Dictionary<int, LogisticsInsuranceType> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static LogisticsInsuranceType GetById(int id)
    {
        if (codeTypeMap.TryGetValue(id, out LogisticsInsuranceType? codeType))
        {
            return codeType;
        }

        return None;
    }

    public static IList<LogisticsInsuranceType> GetList()
    {
        return codeTypeList.ToList();
    }

    private LogisticsInsuranceType(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class LogisticsInsuranceTypeHibernateType : CodeHibernateTypeBase<LogisticsInsuranceType>
{
    protected override LogisticsInsuranceType GetCodeTypeById(int id)
    {
        return LogisticsInsuranceType.GetById(id);
    }
}

public class LogisticsInsuranceTypeModelBinder : CodeTypeModelBinderBase<LogisticsInsuranceType>
{
    protected override LogisticsInsuranceType GetCodeTypeById(int id)
    {
        return LogisticsInsuranceType.GetById(id);
    }
}