using shunda.Core.CodeType;

namespace shunda.Domain.Coupon;

/// <summary>
/// 优惠券获取方式
/// </summary>
public class CouponGetMethod : CodeTypeBase {
    /// <summary>
    /// 客户领取
    /// </summary>
    public static readonly CouponGetMethod Pick = new CouponGetMethod(1000, "客户领取", "客户领取");

    /// <summary>
    /// 后台发放
    /// </summary>
    public static readonly CouponGetMethod Grant = new CouponGetMethod(2000, "后台发放", "后台发放");

    /// <summary>
    /// 关注赠送
    /// </summary>
    public static readonly CouponGetMethod Follow = new CouponGetMethod(3000, "关注赠送", "关注赠送");

    /// <summary>
    /// None
    /// </summary>
    public static readonly CouponGetMethod None = new CouponGetMethod(0, "", "");

    private static readonly IReadOnlyList<CouponGetMethod> codeTypeList = new List<CouponGetMethod>() {
        Pick,
        Grant,
        Follow,
    };

    private static readonly IDictionary<int, CouponGetMethod> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static CouponGetMethod GetById(int id) {
        if (codeTypeMap.TryGetValue(id, out CouponGetMethod? getMethod)) {
            return getMethod;
        }

        return None;
    }

    public static IList<CouponGetMethod> GetList() {
        return codeTypeList.ToList();
    }

    private CouponGetMethod(int id, string name, string description) : base(id, name, description) {
    }
}

public class CouponGetMethodHibernateType : CodeHibernateTypeBase<CouponGetMethod> {
    protected override CouponGetMethod GetCodeTypeById(int id) {
        return CouponGetMethod.GetById(id);
    }
}

public class CouponGetMethodModelBinder : CodeTypeModelBinderBase<CouponGetMethod> {
    protected override CouponGetMethod GetCodeTypeById(int id) {
        return CouponGetMethod.GetById(id);
    }
}