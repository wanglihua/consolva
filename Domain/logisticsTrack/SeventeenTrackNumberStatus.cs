using shunda.Core.CodeType;

namespace shunda.Domain.LogisticsTrack;

/// <summary>
/// 17TRACK注册快递单号状态
/// </summary>
public class SeventeenTrackNumberStatus : CodeTypeBase {
    /// <summary>
    /// 注册成功, 1000
    /// </summary>
    public static readonly SeventeenTrackNumberStatus Accepted = new SeventeenTrackNumberStatus(1000, "注册成功", "注册成功");

    /// <summary>
    /// 注册被拒, 2000
    /// </summary>
    public static readonly SeventeenTrackNumberStatus Rejected = new SeventeenTrackNumberStatus(2000, "注册被拒", "注册被拒");

    /// <summary>
    /// 跟踪完结, 3000
    /// </summary>
    public static readonly SeventeenTrackNumberStatus Completed = new SeventeenTrackNumberStatus(3000, "跟踪完结", "跟踪完结");

    /// <summary>
    /// None
    /// </summary>
    public static readonly SeventeenTrackNumberStatus None = new SeventeenTrackNumberStatus(0, "", "");

    private static readonly IReadOnlyList<SeventeenTrackNumberStatus> codeTypeList = new List<SeventeenTrackNumberStatus>() {
        Accepted,
        Rejected,
        Completed,
    };

    private static readonly Dictionary<int, SeventeenTrackNumberStatus> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static SeventeenTrackNumberStatus GetById(int id) {
        if (codeTypeMap.TryGetValue(id, out SeventeenTrackNumberStatus? codeType)) {
            return codeType;
        }

        return None;
    }

    public static List<SeventeenTrackNumberStatus> GetList() {
        return codeTypeList.ToList();
    }

    private SeventeenTrackNumberStatus(int id, string name, string description) : base(id, name, description) {
    }
}

public class SeventeenTrackNumberStatusHibernateType : CodeHibernateTypeBase<SeventeenTrackNumberStatus> {
    protected override SeventeenTrackNumberStatus GetCodeTypeById(int id) {
        return SeventeenTrackNumberStatus.GetById(id);
    }
}

public class SeventeenTrackNumberStatusModelBinder : CodeTypeModelBinderBase<SeventeenTrackNumberStatus> {
    protected override SeventeenTrackNumberStatus GetCodeTypeById(int id) {
        return SeventeenTrackNumberStatus.GetById(id);
    }
}
