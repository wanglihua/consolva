using shunda.Core.CodeType;

namespace shunda.Domain.Customer;

/// <summary>
/// 会员充值方式
/// </summary>
public class CustomerChargeType : CodeTypeBase
{
    /// <summary>
    /// 微信支付
    /// </summary>
    public static readonly CustomerChargeType Wechat = new CustomerChargeType(1000, "微信", "微信支付");

    /// <summary>
    /// 支付宝支付
    /// </summary>
    public static readonly CustomerChargeType AliPay = new CustomerChargeType(2000, "支付宝", "支付宝支付");

    /// <summary>
    /// PayPal支付(澳币)
    /// </summary>
    public static readonly CustomerChargeType PayPal = new CustomerChargeType(3000, "PayPal", "PayPal支付");

    /// <summary>
    /// 后台手动充值
    /// </summary>
    public static readonly CustomerChargeType Manual = new CustomerChargeType(9000, "手工充值", "手工充值");

    /// <summary>
    /// None
    /// </summary>
    public static readonly CustomerChargeType None = new CustomerChargeType(0, "", "");


    private static List<CustomerChargeType> codeTypeList = new List<CustomerChargeType>() {
        Wechat,
        AliPay,
        PayPal,
        Manual,
    };

    private static Dictionary<int, CustomerChargeType> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static CustomerChargeType GetById(int id)
    {
        if (codeTypeMap.TryGetValue(id, out CustomerChargeType? codeType) == true)
        {
            return codeType;
        }

        return None;
    }

    /// <summary>
    /// 返回所有支付类型列表
    /// </summary>
    /// <returns></returns>
    public static IList<CustomerChargeType> GetList()
    {
        return codeTypeList.ToList();
    }

    private CustomerChargeType(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class CustomerChargeTypeHibernateType : CodeHibernateTypeBase<CustomerChargeType>
{
    protected override CustomerChargeType GetCodeTypeById(int id)
    {
        return CustomerChargeType.GetById(id);
    }
}

public class CustomerChargeTypeModelBinder : CodeTypeModelBinderBase<CustomerChargeType>
{
    protected override CustomerChargeType GetCodeTypeById(int id)
    {
        return CustomerChargeType.GetById(id);
    }
}