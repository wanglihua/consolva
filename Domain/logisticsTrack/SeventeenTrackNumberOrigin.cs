using shunda.Core.CodeType;

namespace shunda.Domain.LogisticsTrack;

/// <summary>
/// 17TRACK运输商origin
/// </summary>
public class SeventeenTrackNumberOrigin : CodeTypeBase {
    /// <summary>
    /// 准确识别, 1
    /// </summary>
    public static readonly SeventeenTrackNumberOrigin Accurate = new SeventeenTrackNumberOrigin(1, "准确识别", "准确识别");

    /// <summary>
    /// 用户输入, 2
    /// </summary>
    public static readonly SeventeenTrackNumberOrigin UserInput = new SeventeenTrackNumberOrigin(2, "用户输入", "用户输入");

    /// <summary>
    /// 自动检测, 3
    /// </summary>
    public static readonly SeventeenTrackNumberOrigin AutoDetect = new SeventeenTrackNumberOrigin(3, "自动检测", "自动检测");

    /// <summary>
    /// None
    /// </summary>
    public static readonly SeventeenTrackNumberOrigin None = new SeventeenTrackNumberOrigin(0, "", "");

    private static readonly IReadOnlyList<SeventeenTrackNumberOrigin> codeTypeList = new List<SeventeenTrackNumberOrigin>() {
        Accurate,
        UserInput,
        AutoDetect,
    };

    private static readonly Dictionary<int, SeventeenTrackNumberOrigin> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static SeventeenTrackNumberOrigin GetById(int id) {
        if (codeTypeMap.TryGetValue(id, out SeventeenTrackNumberOrigin? codeType)) {
            return codeType;
        }

        return None;
    }

    public static List<SeventeenTrackNumberOrigin> GetList() {
        return codeTypeList.ToList();
    }

    private SeventeenTrackNumberOrigin(int id, string name, string description) : base(id, name, description) {
    }
}

public class SeventeenTrackNumberOriginHibernateType : CodeHibernateTypeBase<SeventeenTrackNumberOrigin> {
    protected override SeventeenTrackNumberOrigin GetCodeTypeById(int id) {
        return SeventeenTrackNumberOrigin.GetById(id);
    }
}

public class SeventeenTrackNumberOriginModelBinder : CodeTypeModelBinderBase<SeventeenTrackNumberOrigin> {
    protected override SeventeenTrackNumberOrigin GetCodeTypeById(int id) {
        return SeventeenTrackNumberOrigin.GetById(id);
    }
}
