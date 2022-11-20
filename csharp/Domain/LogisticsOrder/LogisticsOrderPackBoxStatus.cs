using shunda.Core.CodeType;

namespace shunda.Domain.LogisticsOrder;

/// <summary>
/// 订单打包箱号状态
/// </summary>
public class LogisticsOrderPackBoxStatus : CodeTypeBase
{
    /// <summary>
    /// 已打包
    /// </summary>
    public static readonly LogisticsOrderPackBoxStatus Pack = new LogisticsOrderPackBoxStatus(1000, "已打包", "已打包");

    /// <summary>
    /// 已发货
    /// </summary>
    public static readonly LogisticsOrderPackBoxStatus Ship = new LogisticsOrderPackBoxStatus(2000, "已发货", "已发货");

    /// <summary>
    /// 已交货
    /// </summary>
    public static readonly LogisticsOrderPackBoxStatus Deliver = new LogisticsOrderPackBoxStatus(3000, "已交货", "已交货");

    /// <summary>
    /// None
    /// </summary>
    private static readonly LogisticsOrderPackBoxStatus None = new LogisticsOrderPackBoxStatus(0, "", "");

    private static IReadOnlyList<LogisticsOrderPackBoxStatus> codeTypeList = new List<LogisticsOrderPackBoxStatus>() {
        Pack,
        Ship,
        Deliver,
    };

    private static Dictionary<int, LogisticsOrderPackBoxStatus> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static LogisticsOrderPackBoxStatus GetById(int id)
    {
        if (codeTypeMap.TryGetValue(id, out LogisticsOrderPackBoxStatus? codeType))
        {
            return codeType;
        }

        return None;
    }

    public static IList<LogisticsOrderPackBoxStatus> GetList()
    {
        return codeTypeList.ToList();
    }

    private LogisticsOrderPackBoxStatus(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class LogisticsOrderPackBoxStatusHibernateType : CodeHibernateTypeBase<LogisticsOrderPackBoxStatus>
{
    protected override LogisticsOrderPackBoxStatus GetCodeTypeById(int id)
    {
        return LogisticsOrderPackBoxStatus.GetById(id);
    }
}

public class LogisticsOrderPackBoxStatusModelBinder : CodeTypeModelBinderBase<LogisticsOrderPackBoxStatus>
{
    protected override LogisticsOrderPackBoxStatus GetCodeTypeById(int id)
    {
        return LogisticsOrderPackBoxStatus.GetById(id);
    }
}