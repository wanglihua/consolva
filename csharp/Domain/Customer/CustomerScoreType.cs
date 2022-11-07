using shunda.Core.CodeType;

namespace shunda.Domain.Customer;

/// <summary>
/// 客户积分类型
/// </summary>
public class CustomerScoreType : CodeTypeBase
{
    /// <summary>
    /// 会员推广
    /// </summary>
    public static readonly CustomerScoreType Promotion = new CustomerScoreType(1000, "会员推广", "会员推广");

    /// <summary>
    /// 集运消费
    /// </summary>
    public static readonly CustomerScoreType LogisticsConsume = new CustomerScoreType(2000, "集运消费", "集运消费");

    /// <summary>
    /// 积分换余额
    /// </summary>
    public static readonly CustomerScoreType ScoreToBalance = new CustomerScoreType(3000, "积分换余额", "积分换余额");

    /// <summary>
    /// None
    /// </summary>
    public static readonly CustomerScoreType None = new CustomerScoreType(0, "", "");

    private static readonly IReadOnlyList<CustomerScoreType> codeTypeList = new List<CustomerScoreType>() {
        Promotion,
        LogisticsConsume,
        ScoreToBalance,
    };

    private static readonly Dictionary<int, CustomerScoreType> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static CustomerScoreType GetById(int id)
    {
        if(codeTypeMap.TryGetValue(id, out CustomerScoreType? codeType))
        {
            return codeType;
        }

        return None;
    }

    public static IList<CustomerScoreType> GetList()
    {
        return codeTypeList.ToList();
    }

    private CustomerScoreType(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class CustomerScoreTypeHibernateType : CodeHibernateTypeBase<CustomerScoreType>
{
    protected override CustomerScoreType GetCodeTypeById(int id)
    {
        return CustomerScoreType.GetById(id);
    }
}

public class CustomerScoreTypeModelBinder : CodeTypeModelBinderBase<CustomerScoreType>
{
    protected override CustomerScoreType GetCodeTypeById(int id)
    {
        return CustomerScoreType.GetById(id);
    }
}
