using shunda.Core.CodeType;

namespace shunda.Domain.Coupon;

/// <summary>
/// 优惠券获取方式
/// </summary>
public class CouponGetMethod : CodeTypeBase
{
    /// <summary>
    /// 客户领取
    /// </summary>
    public static readonly CouponGetMethod Pick = new CouponGetMethod(1000, "客户领取", "客户领取");

    /// <summary>
    /// 后台发放
    /// </summary>
    public static readonly CouponGetMethod Grant = new CouponGetMethod(2000, "后台发放", "后台发放");

    /// <summary>
    /// None
    /// </summary>
    public static readonly CouponGetMethod None = new CouponGetMethod(0, "", "");

    public static CouponGetMethod GetById(int id)
    {
        IList<CouponGetMethod> typeList = GetList();
        foreach (CouponGetMethod type in typeList)
        {
            if (type.Id == id)
            {
                return type;
            }
        }

        return None;
    }

    public static IList<CouponGetMethod> GetList()
    {
        IList<CouponGetMethod> typeList = new List<CouponGetMethod>();

        typeList.Add(Pick);
        typeList.Add(Grant);

        return typeList;
    }

    private CouponGetMethod(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class CouponGetMethodHibernateType : CodeHibernateTypeBase<CouponGetMethod>
{
    protected override CouponGetMethod GetCodeTypeById(int id)
    {
        return CouponGetMethod.GetById(id);
    }
}

public class CouponGetMethodModelBinder : CodeTypeModelBinderBase<CouponGetMethod>
{
    protected override CouponGetMethod GetCodeTypeById(int id)
    {
        return CouponGetMethod.GetById(id);
    }
}