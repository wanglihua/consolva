using shunda.Core.CodeType;

namespace shunda.Domain.Coupon;

/// <summary>
/// 优惠券状态
/// </summary>
public class CouponStatus : CodeTypeBase
{
    /// <summary>
    /// 已领取
    /// </summary>
    public static readonly CouponStatus Pick = new CouponStatus(1000, "已领取", "已领取");

    /// <summary>
    /// 已使用
    /// </summary>
    public static readonly CouponStatus Use = new CouponStatus(2000, "已使用", "已使用");

    /// <summary>
    /// 已过期
    /// </summary>
    public static readonly CouponStatus Expire = new CouponStatus(3000, "已过期", "已过期");

    /// <summary>
    /// None
    /// </summary>
    public static readonly CouponStatus None = new CouponStatus(0, "", "");

    public static CouponStatus GetById(int id)
    {
        IList<CouponStatus> typeList = GetList();
        foreach (CouponStatus type in typeList)
        {
            if (type.Id == id)
            {
                return type;
            }
        }

        return None;
    }

    public static IList<CouponStatus> GetList()
    {
        IList<CouponStatus> typeList = new List<CouponStatus>();

        typeList.Add(Pick);
        typeList.Add(Use);
        typeList.Add(Expire);

        return typeList;
    }

    private CouponStatus(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class CouponStatusHibernateType : CodeHibernateTypeBase<CouponStatus>
{
    protected override CouponStatus GetCodeTypeById(int id)
    {
        return CouponStatus.GetById(id);
    }
}

public class CouponStatusModelBinder : CodeTypeModelBinderBase<CouponStatus>
{
    protected override CouponStatus GetCodeTypeById(int id)
    {
        return CouponStatus.GetById(id);
    }
}