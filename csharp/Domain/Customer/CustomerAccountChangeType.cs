using shunda.Core.CodeType;

namespace shunda.Domain.Customer;

/// <summary>
///客户余额变动类型
/// </summary>
public class CustomerAccountChangeType : CodeTypeBase
{
    /// <summary>
    /// 余额充值
    /// </summary>
    public static readonly CustomerAccountChangeType Charge = new CustomerAccountChangeType(100, "余额充值", "余额充值");

    /// <summary>
    /// 余额提现
    /// </summary>
    public static readonly CustomerAccountChangeType Withdraw = new CustomerAccountChangeType(200, "余额提现", "余额提现");

    /// <summary>
    /// 余额支付
    /// </summary>
    public static readonly CustomerAccountChangeType Pay = new CustomerAccountChangeType(300, "余额支付", "余额支付");

    /// <summary>
    /// 余额代支付
    /// </summary>
    public static readonly CustomerAccountChangeType PayAnother = new CustomerAccountChangeType(350, "余额代支付", "余额代支付");

    /// <summary>
    /// 注册奖励
    /// </summary>
    public static readonly CustomerAccountChangeType RegisterAward = new CustomerAccountChangeType(400, "注册奖励", "注册奖励");

    /// <summary>
    /// 积分兑换余额
    /// </summary>
    public static readonly CustomerAccountChangeType ScoreToAccount = new CustomerAccountChangeType(500, "积分兑换余额", "积分兑换余额");

    /// <summary>
    /// 充值优惠
    /// </summary>
    public static readonly CustomerAccountChangeType ChargeAward = new CustomerAccountChangeType(600, "充值优惠", "充值优惠");

    /// <summary>
    /// 退赔款
    /// </summary>
    public static readonly CustomerAccountChangeType Refund = new CustomerAccountChangeType(700, "退赔款", "退赔款");

    /// <summary>
    /// None
    /// </summary>
    public static readonly CustomerAccountChangeType None = new CustomerAccountChangeType(0, "", "");

    private static readonly List<CustomerAccountChangeType> codeTypeList = new List<CustomerAccountChangeType>() {
        Charge,
        Withdraw,
        Pay,
        PayAnother,
        RegisterAward,
        ScoreToAccount,
        ChargeAward,
        Refund
    };

    private static readonly Dictionary<int, CustomerAccountChangeType> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static CustomerAccountChangeType GetById(int id)
    {
        if (codeTypeMap.TryGetValue(id, out CustomerAccountChangeType? codeType))
        {
            return codeType;
        }

        return None;
    }

    public static IList<CustomerAccountChangeType> GetList()
    {
        return codeTypeList;
    }

    private CustomerAccountChangeType(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class CustomerAccountChangeTypeHibernateType : CodeHibernateTypeBase<CustomerAccountChangeType>
{
    protected override CustomerAccountChangeType GetCodeTypeById(int id)
    {
        return CustomerAccountChangeType.GetById(id);
    }
}

public class CustomerAccountChangeTypeModelBinder : CodeTypeModelBinderBase<CustomerAccountChangeType>
{
    protected override CustomerAccountChangeType GetCodeTypeById(int id)
    {
        return CustomerAccountChangeType.GetById(id);
    }
}