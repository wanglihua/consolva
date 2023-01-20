using shunda.Core.CodeType;

namespace shunda.Domain;

/// <summary>
/// 时间段类型
/// </summary>
public class TimeRangeType : CodeTypeBase {
    /// <summary>
    /// 订单创建时间
    /// </summary>
    public static readonly TimeRangeType OrderCreate = new TimeRangeType(100, "订单创建时间", "订单创建时间");

    /// <summary>
    /// 提交打包时间
    /// </summary>
    public static readonly TimeRangeType SubmitPack = new TimeRangeType(200, "提交打包时间", "提交打包时间");

    /// <summary>
    /// 完成打包时间
    /// </summary>
    public static readonly TimeRangeType CompletePack = new TimeRangeType(300, "完成打包时间", "完成打包时间");

    /// <summary>
    /// 付款时间
    /// </summary>
    public static readonly TimeRangeType Pay = new TimeRangeType(400, "付款时间", "付款时间");

    /// <summary>
    /// 出库时间
    /// </summary>
    public static readonly TimeRangeType Checkout = new TimeRangeType(430, "出库时间", "出库时间");

    /// <summary>
    /// 签收时间
    /// </summary>
    public static readonly TimeRangeType Deliver = new TimeRangeType(500, "签收时间", "签收时间");

    /// <summary>
    /// None
    /// </summary>
    public static readonly TimeRangeType None = new TimeRangeType(0, "", "");

    private static readonly IReadOnlyList<TimeRangeType> codeTypeList = new List<TimeRangeType>() {
        OrderCreate,
        SubmitPack,
        CompletePack,
        Pay,
        Checkout,
        Deliver,
    };

    private static readonly IDictionary<int, TimeRangeType> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static TimeRangeType GetById(int id) {
        if (codeTypeMap.TryGetValue(id, out TimeRangeType? codeType)) {
            return codeType;
        }

        return None;
    }

    public static IList<TimeRangeType> GetList() {
        return codeTypeList.ToList();
    }

    private TimeRangeType(int id, string name, string description) : base(id, name, description) {
    }
}

public class TimeRangeTypeHibernateType : CodeHibernateTypeBase<TimeRangeType> {
    protected override TimeRangeType GetCodeTypeById(int id) {
        return TimeRangeType.GetById(id);
    }
}

public class TimeRangeTypeModelBinder : CodeTypeModelBinderBase<TimeRangeType> {
    protected override TimeRangeType GetCodeTypeById(int id) {
        return TimeRangeType.GetById(id);
    }
}