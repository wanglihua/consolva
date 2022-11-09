using shunda.Core.CodeType;

namespace shunda.Domain.Fee;

public class FeeRefundPayType : CodeTypeBase
{
    /// <summary>
    /// 支付宝
    /// </summary>
    public static readonly FeeRefundPayType AliPay = new FeeRefundPayType(100, "支付宝", "支付宝");

    /// <summary>
    /// 微信
    /// </summary>
    public static readonly FeeRefundPayType WxPay = new FeeRefundPayType(200, "微信", "微信");

    /// <summary>
    /// 转账
    /// </summary>
    public static readonly FeeRefundPayType Transfer = new FeeRefundPayType(300, "转账", "转账");

    /// <summary>
    /// 余额充值
    /// </summary>
    public static readonly FeeRefundPayType Charge = new FeeRefundPayType(400, "余额充值", "余额充值");

    /// <summary>
    /// None
    /// </summary>
    public static readonly FeeRefundPayType None = new FeeRefundPayType(0, "None", "None");


    public static FeeRefundPayType GetById(int typeId)
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

    private static IList<FeeRefundPayType> typeList = new List<FeeRefundPayType>() { AliPay, WxPay, Transfer, Charge };

    public static IList<FeeRefundPayType> GetList()
    {
        return typeList;
    }

    private FeeRefundPayType(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class FeeRefundPayTypeHibernateType : CodeHibernateTypeBase<FeeRefundPayType>
{
    protected override FeeRefundPayType GetCodeTypeById(int id)
    {
        return FeeRefundPayType.GetById(id);
    }
}

public class FeeRefundPayTypeModelBinder : CodeTypeModelBinderBase<FeeRefundPayType>
{
    protected override FeeRefundPayType GetCodeTypeById(int id)
    {
        return FeeRefundPayType.GetById(id);
    }
}