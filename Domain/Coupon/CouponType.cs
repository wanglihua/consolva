using shunda.Core.CodeType;

namespace shunda.Domain.Coupon;

/// <summary>
/// 优惠券类型
/// </summary>
public class CouponType : CodeTypeBase
{
    /// <summary>
    /// 抵扣
    /// </summary>
    public static readonly CouponType Deduct = new CouponType(1000, "A", "抵扣券", "抵扣券");

    /// <summary>
    /// 折扣
    /// </summary>
    public static readonly CouponType Discount = new CouponType(2000, "B", "折扣券", "折扣券");

    /// <summary>
    /// 满减
    /// </summary>
    public static readonly CouponType PriceBreak = new CouponType(3000, "C", "满减券", "满减券");

    /// <summary>
    /// None
    /// </summary>
    public static readonly CouponType None = new CouponType(0, "", "", "");

    public static CouponType GetById(int id)
    {
        IList<CouponType> typeList = GetList();
        foreach (CouponType type in typeList)
        {
            if (type.Id == id)
            {
                return type;
            }
        }

        return None;
    }

    public static IList<CouponType> GetList()
    {
        IList<CouponType> typeList = new List<CouponType>();

        typeList.Add(Deduct);
        typeList.Add(Discount);
        typeList.Add(PriceBreak);

        return typeList;
    }

    private CouponType(int id, string code, string name, string description) : base(id, name, description)
    {
        this.Code = code;
    }

    public string Code { get; set; }
}

public class CouponTypeHibernateType : CodeHibernateTypeBase<CouponType>
{
    protected override CouponType GetCodeTypeById(int id)
    {
        return CouponType.GetById(id);
    }
}

public class CouponTypeModelBinder : CodeTypeModelBinderBase<CouponType>
{
    protected override CouponType GetCodeTypeById(int id)
    {
        return CouponType.GetById(id);
    }
}