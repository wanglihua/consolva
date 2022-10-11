using shunda.Core.CodeType;

namespace shunda.Domain;

public class PayType : CodeTypeBase
{
    /// <summary>
    /// 账户余额支付, 1000
    /// </summary>
    public static readonly PayType Balance = new PayType(1000, "账户余额", "账户余额支付");

    /// <summary>
    /// 微信支付, 2000
    /// </summary>
    public static readonly PayType Wechat = new PayType(2000, "微信", "微信支付");

    /// <summary>
    /// 支付宝支付, 3000
    /// </summary>
    public static readonly PayType AliPay = new PayType(3000, "支付宝", "支付宝支付");

    /// <summary>
    /// PayPal支付, 4000
    /// </summary>
    public static readonly PayType PayPal = new PayType(4000, "PayPal", "PayPal支付");

    /// <summary>
    /// 信用卡直接支付, 5000
    /// </summary>
    public static readonly PayType CreditCard = new PayType(5000, "信用卡", "信用卡直接支付");

    /// <summary>
    /// 其他支付方式, 6000
    /// </summary>
    public static readonly PayType Other = new PayType(6000, "其他", "其他支付方式");

    /// <summary>
    /// 费用核销, 7000
    /// </summary>
    public static readonly PayType WriteOff = new PayType(7000, "费用核销", "支付后，通过费用核销生成支付记录");

    /// <summary>
    /// None
    /// </summary>
    public static readonly PayType None = new PayType(0, "", "");

    private static readonly List<PayType> codeTypeList = new List<PayType>() {
        Balance,
        Wechat,
        AliPay,
        PayPal,
        CreditCard,
        Other,
        WriteOff,
    };

    private static readonly Dictionary<int, PayType> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static PayType GetById(int id)
    {
        if (codeTypeMap.TryGetValue(id, out PayType? codeType))
        {
            return codeType;
        }

        return None;
    }

    /// <summary>
    /// 返回所有支付类型列表
    /// </summary>
    /// <returns></returns>
    public static IList<PayType> GetList()
    {
        return codeTypeList;
    }

    /// <summary>
    /// 获取支付类型列表，用于客户支付时的可选支付类型选择
    /// </summary>
    /// <returns></returns>
    public static IList<PayType> GetCustomerPayTypeList()
    {
        IList<PayType> payTypeList = new List<PayType>();
        payTypeList.Add(Balance);
        payTypeList.Add(Wechat);
        payTypeList.Add(AliPay);
        payTypeList.Add(PayPal);
        payTypeList.Add(CreditCard);

        return payTypeList;
    }

    private PayType(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class PayTypeHibernateType : CodeHibernateTypeBase<PayType>
{
    protected override PayType GetCodeTypeById(int id)
    {
        return PayType.GetById(id);
    }
}

public class PayTypeModelBinder : CodeTypeModelBinderBase<PayType>
{
    protected override PayType GetCodeTypeById(int id)
    {
        return PayType.GetById(id);
    }
}