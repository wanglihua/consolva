using shunda.Core.CodeType;

namespace shunda.Domain;

/// <summary>
/// 支付类别
/// </summary>
public class PayCate : CodeTypeBase
{
    /// <summary>
    /// 订单支付, 1000
    /// </summary>
    public static readonly PayCate Order = new PayCate(1000, "订单支付", "订单支付");

    /// <summary>
    /// 充值支付, 2000
    /// </summary>
    public static readonly PayCate Charge = new PayCate(2000, "充值支付", "充值支付");

    /// <summary>
    /// 快递退运支付, 3000
    /// </summary>
    public static readonly PayCate ExpressReturn = new PayCate(3000, "快递退运支付", "快递退运支付");

    /// <summary>
    /// None
    /// </summary>
    public static readonly PayCate None = new PayCate(0, "", "");


    private static readonly List<PayCate> codeTypeList = new List<PayCate>() {
        Order,
        Charge,
        ExpressReturn,
    };

    private static readonly Dictionary<int, PayCate> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static PayCate GetById(int id)
    {
        if (codeTypeMap.TryGetValue(id, out PayCate? codeType))
        {
            return codeType;
        }

        return None;
    }

    public static IList<PayCate> GetList()
    {
        return codeTypeList;
    }

    private PayCate(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class PayCateHibernateType : CodeHibernateTypeBase<PayCate>
{
    protected override PayCate GetCodeTypeById(int id)
    {
        return PayCate.GetById(id);
    }
}

public class PayCateModelBinder : CodeTypeModelBinderBase<PayCate>
{
    protected override PayCate GetCodeTypeById(int id)
    {
        return PayCate.GetById(id);
    }
}