using shunda.Core.CodeType;

namespace shunda.Domain.Coupon;

/// <summary>
/// 优惠券批次状态
/// </summary>
public class CouponBatchStatus : CodeTypeBase {
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
    /// 这个现在还没用起来 
    /// </summary>
    public static readonly CouponBatchStatus Complete = new CouponBatchStatus(4000, "已完结", "已完结");

    /// <summary>
    /// 已过期
    /// </summary>
    public static readonly CouponBatchStatus Expire = new CouponBatchStatus(5000, "已过期", "已过期");

    /// <summary>
    /// 已停用
    /// 这个状态现在只用在关注公众号优惠券赠送时的判断
    /// </summary>
    public static readonly CouponBatchStatus Disabled = new CouponBatchStatus(6000, "已停用", "已停用");

    /// <summary>
    /// None
    /// </summary>
    public static readonly CouponBatchStatus None = new CouponBatchStatus(0, "", "");

    private static readonly IReadOnlyList<CouponBatchStatus> codeTypeList = new List<CouponBatchStatus>() {
        New,
        Release,
        Picked,
        Complete,
        Expire,
        Disabled,
    };

    private static readonly IDictionary<int, CouponBatchStatus> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static CouponBatchStatus GetById(int id) {
        if (codeTypeMap.TryGetValue(id, out CouponBatchStatus? codeType)) {
            return codeType;
        }

        return None;
    }

    public static IList<CouponBatchStatus> GetList() {
        return codeTypeList.ToList();
    }

    public static IList<CouponBatchStatus> GetMayExpireStatusList() {
        // 当前只有这两种状态下可能会过期
        return new List<CouponBatchStatus>() {
            Release,
            Picked,
        };
    }

    private CouponBatchStatus(int id, string name, string description) : base(id, name, description) {
    }

    public bool Released() {
        if (Id == New.Id) {
            return false;
        }

        return true;
    }
}

public class CouponBatchStatusHibernateType : CodeHibernateTypeBase<CouponBatchStatus> {
    protected override CouponBatchStatus GetCodeTypeById(int id) {
        return CouponBatchStatus.GetById(id);
    }
}

public class CouponBatchStatusModelBinder : CodeTypeModelBinderBase<CouponBatchStatus> {
    protected override CouponBatchStatus GetCodeTypeById(int id) {
        return CouponBatchStatus.GetById(id);
    }
}