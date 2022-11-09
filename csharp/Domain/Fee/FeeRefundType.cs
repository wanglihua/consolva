using shunda.Core.CodeType;

namespace shunda.Domain.Fee;

public class FeeRefundType : CodeTypeBase
{
    /// <summary>
    /// 退款
    /// </summary>
    public static readonly FeeRefundType Refund = new FeeRefundType(100, "退款", "客户支付了比订单当前费用更多的钱，订单处于超额支付状态，审核后会多一条订单费用抵充");

    /// <summary>
    /// 赔款
    /// </summary>
    public static readonly FeeRefundType Indemnity = new FeeRefundType(200, "赔款", "客户没有支付比订单费用更多的钱，但需要给与费用补偿，审核后不会多一条订单费用抵充");


    /// <summary>
    /// None
    /// </summary>
    public static readonly FeeRefundType None = new FeeRefundType(0, "None", "None");


    public static FeeRefundType GetById(int typeId)
    {
        foreach (var typeInfo in GetList())
        {
            if (typeInfo.Id == typeId)
            {
                return typeInfo;
            }
        }

        return None;
    }

    private static IList<FeeRefundType> typeList = new List<FeeRefundType>() { Refund, Indemnity };

    public static IList<FeeRefundType> GetList()
    {
        return typeList;
    }

    private FeeRefundType(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class FeeRefundTypeHibernateType : CodeHibernateTypeBase<FeeRefundType>
{
    protected override FeeRefundType GetCodeTypeById(int id)
    {
        return FeeRefundType.GetById(id);
    }
}

public class FeeRefundTypeModelBinder : CodeTypeModelBinderBase<FeeRefundType>
{
    protected override FeeRefundType GetCodeTypeById(int id)
    {
        return FeeRefundType.GetById(id);
    }
}