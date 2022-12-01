using shunda.Core.CodeType;

namespace shunda.Domain.Coupon;

/// <summary>
/// 优惠券批次状态
/// </summary>
public class CouponBatchStatus : CodeTypeBase
{
    /// <summary>
    /// 新创建
    /// </summary>
    public static readonly CouponBatchStatus New = new CouponBatchStatus(1000, "新创建", "新创建");

    /// <summary>
    /// 已投放
    /// </summary>
    public static readonly CouponBatchStatus Release = new CouponBatchStatus(2000, "已投放", "已投放");

    /// <summary>
    /// 已领完
    /// </summary>
    public static readonly CouponBatchStatus Picked = new CouponBatchStatus(3000, "已领完", "已领完");

    /// <summary>
    /// 已完结
    /// 这个状态现在还没用起来
    /// 实际应该在使用优惠券时判断，在订单取消时判断，或者满减券计费时发现无效时
    /// </summary>
    public static readonly CouponBatchStatus Complete = new CouponBatchStatus(4000, "已完结", "已完结");

    /// <summary>
    /// 已过期
    /// </summary>
    public static readonly CouponBatchStatus Expire = new CouponBatchStatus(5000, "已过期", "已过期");

    /// <summary>
    /// None
    /// </summary>
    public static readonly CouponBatchStatus None = new CouponBatchStatus(0, "", "");

    public static CouponBatchStatus GetById(int id)
    {
        IList<CouponBatchStatus> typeList = GetList();
        foreach (CouponBatchStatus type in typeList)
        {
            if (type.Id == id)
            {
                return type;
            }
        }

        return None;
    }

    public static IList<CouponBatchStatus> GetList()
    {
        IList<CouponBatchStatus> typeList = new List<CouponBatchStatus>();

        typeList.Add(New);
        typeList.Add(Release);
        typeList.Add(Picked);
        typeList.Add(Complete);
        typeList.Add(Expire);

        return typeList;
    }

    public static IList<CouponBatchStatus> GetMayExpireStatusList()
    {
        IList<CouponBatchStatus> typeList = new List<CouponBatchStatus>();

        // 当前只有这两种状态下可能会过期
        typeList.Add(Release);
        typeList.Add(Picked);

        return typeList;
    }

    private CouponBatchStatus(int id, string name, string description) : base(id, name, description)
    {
    }

    public bool Released()
    {
        if (Id == New.Id)
        {
            return false;
        }

        return true;
    }
}

public class CouponBatchStatusHibernateType : CodeHibernateTypeBase<CouponBatchStatus>
{
    protected override CouponBatchStatus GetCodeTypeById(int id)
    {
        return CouponBatchStatus.GetById(id);
    }
}

public class CouponBatchStatusModelBinder : CodeTypeModelBinderBase<CouponBatchStatus>
{
    protected override CouponBatchStatus GetCodeTypeById(int id)
    {
        return CouponBatchStatus.GetById(id);
    }
}