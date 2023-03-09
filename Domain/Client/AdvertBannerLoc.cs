using shunda.Core.CodeType;

namespace shunda.Domain.Client;

/// <summary>
/// 广告横幅位置
/// </summary>
public class AdvertBannerLoc : CodeTypeBase {
    /// <summary>
    /// 首页顶部
    /// </summary>
    public static readonly AdvertBannerLoc HomeTop = new AdvertBannerLoc(100, "首页顶部", "首页顶部");

    /// <summary>
    /// None
    /// </summary>
    public static readonly AdvertBannerLoc None = new AdvertBannerLoc(0, "", "");

    private static readonly IReadOnlyList<AdvertBannerLoc> codeTypeList = new List<AdvertBannerLoc>() {
        HomeTop,
    };

    private static readonly IDictionary<int, AdvertBannerLoc> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static AdvertBannerLoc GetById(int id) {
        if (codeTypeMap.TryGetValue(id, out AdvertBannerLoc? codeType)) {
            return codeType;
        }

        return None;
    }

    public static IList<AdvertBannerLoc> GetList() {
        return codeTypeList.ToList();
    }

    private AdvertBannerLoc(int id, string name, string description) : base(id, name, description) {
    }
}

public class AdvertBannerLocHibernateType : CodeHibernateTypeBase<AdvertBannerLoc> {
    protected override AdvertBannerLoc GetCodeTypeById(int id) {
        return AdvertBannerLoc.GetById(id);
    }
}

public class AdvertBannerLocModelBinder : CodeTypeModelBinderBase<AdvertBannerLoc> {
    protected override AdvertBannerLoc GetCodeTypeById(int id) {
        return AdvertBannerLoc.GetById(id);
    }
}