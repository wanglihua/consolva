using shunda.Core.CodeType;

namespace shunda.Domain.Pass;

/// <summary>
/// 仓库工作人员类型
/// </summary>
public class WarehouseWorkerType : CodeTypeBase
{
    /// <summary>
    /// 入库人员
    /// </summary>
    public static readonly WarehouseWorkerType Checkin = new WarehouseWorkerType(100, "入库人员", "入库人员");

    /// <summary>
    /// 上架人员
    /// </summary>
    public static readonly WarehouseWorkerType OnShelf = new WarehouseWorkerType(120, "上架人员", "上架人员");

    /// <summary>
    /// 下架人员
    /// </summary>
    public static readonly WarehouseWorkerType OffShelf = new WarehouseWorkerType(140, "下架人员", "下架人员");

    /// <summary>
    /// 堆存人员
    /// </summary>
    public static readonly WarehouseWorkerType ExpressDump = new WarehouseWorkerType(160, "堆存人员", "快递堆存人员");

    /// <summary>
    /// 打包人员
    /// </summary>
    public static readonly WarehouseWorkerType Pack = new WarehouseWorkerType(200, "打包人员", "打包人员");

    /// <summary>
    /// 打包排班人员
    /// </summary>
    public static readonly WarehouseWorkerType PackSchedule = new WarehouseWorkerType(220, "打包排班人员", "打包排班人员");

    /// <summary>
    /// 发货人员
    /// </summary>
    public static readonly WarehouseWorkerType Ship = new WarehouseWorkerType(240, "发货人员", "发货人员");

    /// <summary>
    /// None
    /// </summary>
    public static readonly WarehouseWorkerType None = new WarehouseWorkerType(0, "", "");

    private static readonly IReadOnlyList<WarehouseWorkerType> codeTypeList = new List<WarehouseWorkerType>() {
        Checkin,
        OnShelf,
        OffShelf,
        ExpressDump,
        Pack,
        PackSchedule,
        Ship,
    };

    private static readonly Dictionary<int, WarehouseWorkerType> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static WarehouseWorkerType GetById(int id)
    {
        if (codeTypeMap.TryGetValue(id, out WarehouseWorkerType? codeType))
        {
            return codeType;
        }

        return None;
    }

    public static IList<WarehouseWorkerType> GetList()
    {
        return codeTypeList.ToList();
    }

    private WarehouseWorkerType(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class WarehouseWorkerTypeHibernateType : CodeHibernateTypeBase<WarehouseWorkerType>
{
    protected override WarehouseWorkerType GetCodeTypeById(int id)
    {
        return WarehouseWorkerType.GetById(id);
    }
}

public class WarehouseWorkerTypeModelBinder : CodeTypeModelBinderBase<WarehouseWorkerType>
{
    protected override WarehouseWorkerType GetCodeTypeById(int id)
    {
        return WarehouseWorkerType.GetById(id);
    }
}