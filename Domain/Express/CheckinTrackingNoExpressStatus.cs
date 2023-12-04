using shunda.Core.CodeType;

namespace shunda.Domain.Express;

/// <summary>
/// 澳拼跟踪单号快递状态
/// </summary>
public class CheckinTrackingNoExpressStatus : CodeTypeBase {
    /// <summary>
    /// 待上架
    /// </summary>
    public static readonly CheckinTrackingNoExpressStatus ToOnShelf = new CheckinTrackingNoExpressStatus(1000, "待上架", "待上架");

    /// <summary>
    /// 已上架
    /// </summary>
    public static readonly CheckinTrackingNoExpressStatus OnShelf = new CheckinTrackingNoExpressStatus(2000, "已上架", "已上架");

    /// <summary>
    /// 已下架
    /// </summary>
    public static readonly CheckinTrackingNoExpressStatus OffShelf = new CheckinTrackingNoExpressStatus(3000, "已下架", "已下架");

    /// <summary>
    /// 已入流动仓
    /// </summary>
    public static readonly CheckinTrackingNoExpressStatus Packing = new CheckinTrackingNoExpressStatus(4000, "已入流动仓", "已入流动仓");

    /// <summary>
    /// 已打包
    /// 用于自动打包时的状态设定
    /// </summary>
    public static readonly CheckinTrackingNoExpressStatus Packed = new CheckinTrackingNoExpressStatus(5000, "已打包", "已打包");

    /// <summary>
    /// 已打包校对
    /// </summary>
    public static readonly CheckinTrackingNoExpressStatus PackCheck = new CheckinTrackingNoExpressStatus(6000, "已打包校对", "已打包校对");

    /// <summary>
    /// None
    /// </summary>
    public static readonly CheckinTrackingNoExpressStatus None = new CheckinTrackingNoExpressStatus(0, "", "");

    private static readonly IReadOnlyList<CheckinTrackingNoExpressStatus> codeTypeList = new List<CheckinTrackingNoExpressStatus>() {
        ToOnShelf,
        OnShelf,
        OffShelf,
        Packing,
        Packed,
        PackCheck,
    };

    private static readonly IDictionary<int, CheckinTrackingNoExpressStatus> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);


    public static CheckinTrackingNoExpressStatus GetById(int id) {
        if (codeTypeMap.TryGetValue(id, out CheckinTrackingNoExpressStatus? codeType)) {
            return codeType;
        }

        return None;
    }

    public static IList<CheckinTrackingNoExpressStatus> GetList() {
        return codeTypeList.ToList();
    }

    public static IList<CheckinTrackingNoExpressStatus> GetOnShelfList() {
        return new List<CheckinTrackingNoExpressStatus> {
            OnShelf,
            OffShelf,
            Packing,
            Packed,
            PackCheck,
        };
    }

    public static IList<CheckinTrackingNoExpressStatus> GetOffShelfList() {
        return new List<CheckinTrackingNoExpressStatus> {
            OffShelf,
            Packing,
            Packed,
            PackCheck,
        };
    }

    public static IList<CheckinTrackingNoExpressStatus> GetPackingList() {
        return new List<CheckinTrackingNoExpressStatus> {
            Packing,
            Packed,
            PackCheck,
        };
    }

    public bool IsPackChecked() {
        if (this.Id == PackCheck.Id) {
            return true;
        }

        return false;
    }

    private CheckinTrackingNoExpressStatus(int id, string name, string description) : base(id, name, description) {
    }
}

public class AoPinTrackingNoExpressStatusHibernateType : CodeHibernateTypeBase<CheckinTrackingNoExpressStatus> {
    protected override CheckinTrackingNoExpressStatus GetCodeTypeById(int id) {
        return CheckinTrackingNoExpressStatus.GetById(id);
    }
}

public class AoPinTrackingNoExpressStatusModelBinder : CodeTypeModelBinderBase<CheckinTrackingNoExpressStatus> {
    protected override CheckinTrackingNoExpressStatus GetCodeTypeById(int id) {
        return CheckinTrackingNoExpressStatus.GetById(id);
    }
}