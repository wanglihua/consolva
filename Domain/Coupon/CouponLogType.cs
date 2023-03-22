using shunda.Core.CodeType;

namespace shunda.Domain.Coupon;

/// <summary>
/// 优惠券日志类型
/// </summary>
public class CouponLogType : CodeTypeBase {
    /// <summary>
    /// 领取, 1000
    /// </summary>
    public static readonly CouponLogType Pick = new CouponLogType(1000, "领取", "领取");

    /// <summary>
    /// 发放, 2000
    /// </summary>
    public static readonly CouponLogType Grant = new CouponLogType(2000, "发放", "发放");

    /// <summary>
    /// 关注赠送, 2500
    /// </summary>
    public static readonly CouponLogType Follow = new CouponLogType(2500, "关注赠送", "关注赠送");

    /// <summary>
    /// 使用, 3000
    /// </summary>
    public static readonly CouponLogType Use = new CouponLogType(3000, "使用", "使用");

    /// <summary>
    /// 撤回, 4000
    /// </summary>
    public static readonly CouponLogType Back = new CouponLogType(4000, "撤回", "撤回");

    /// <summary>
    /// None
    /// </summary>
    public static readonly CouponLogType None = new CouponLogType(0, "", "");

    private static readonly IReadOnlyList<CouponLogType> codeTypeList = new List<CouponLogType>() {
        Pick,
        Grant,
        Follow,
        Use,
        Back,
    };

    private static readonly Dictionary<int, CouponLogType> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static CouponLogType GetById(int id) {
        if (codeTypeMap.TryGetValue(id, out CouponLogType? codeType)) {
            return codeType;
        }

        return None;
    }

    public static List<CouponLogType> GetList() {
        return codeTypeList.ToList();
    }

    private CouponLogType(int id, string name, string description) : base(id, name, description) {
    }
}

public class CouponLogTypeHibernateType : CodeHibernateTypeBase<CouponLogType> {
    protected override CouponLogType GetCodeTypeById(int id) {
        return CouponLogType.GetById(id);
    }
}

public class CouponLogTypeModelBinder : CodeTypeModelBinderBase<CouponLogType> {
    protected override CouponLogType GetCodeTypeById(int id) {
        return CouponLogType.GetById(id);
    }
}