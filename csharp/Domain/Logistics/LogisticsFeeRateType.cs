using shunda.Core.CodeType;

namespace shunda.Domain.Logistics;

public class LogisticsFeeRateType : CodeTypeBase
{
    /// <summary>
    /// 内部结算费率
    /// </summary>
    public static readonly LogisticsFeeRateType Inner = new LogisticsFeeRateType(100, "内部结算费率", "内部结算费率");

    /// <summary>
    /// 终端客户费率
    /// </summary>
    public static readonly LogisticsFeeRateType Customer = new LogisticsFeeRateType(200, "终端客户费率", "终端客户费率");

    /// <summary>
    /// None
    /// </summary>
    private static readonly LogisticsFeeRateType None = new LogisticsFeeRateType(0, "", "");

    private static readonly List<LogisticsFeeRateType> codeTypeList = new List<LogisticsFeeRateType>() {
        Inner,
        Customer,
    };

    private static readonly Dictionary<int, LogisticsFeeRateType> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    /// <summary>
    /// 获取类型列表
    /// </summary>
    /// <returns></returns>
    public static IList<LogisticsFeeRateType> GetList()
    {
        return codeTypeList;
    }

    /// <summary>
    /// 根据 Id 获取费率类型
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static LogisticsFeeRateType GetById(int id)
    {
        if (codeTypeMap.ContainsKey(id))
        {
            return codeTypeMap[id];
        }

        return None;
    }

    private LogisticsFeeRateType(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class LogisticsFeeRateTypeHibernateType : CodeHibernateTypeBase<LogisticsFeeRateType>
{
    protected override LogisticsFeeRateType GetCodeTypeById(int id)
    {
        return LogisticsFeeRateType.GetById(id);
    }
}

public class LogisticsFeeRateTypeModelBinder : CodeTypeModelBinderBase<LogisticsFeeRateType>
{
    protected override LogisticsFeeRateType GetCodeTypeById(int id)
    {
        return LogisticsFeeRateType.GetById(id);
    }
}