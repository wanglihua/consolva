using shunda.Core.CodeType;

namespace shunda.Domain.Storage;

/// <summary>
/// 仓库状态
/// </summary>
public class WarehouseStatus : CodeTypeBase
{
    /// <summary>
    /// 启用
    /// </summary>
    public static readonly WarehouseStatus Enabled = new WarehouseStatus(100, "启用", "启用");

    /// <summary>
    /// 禁用
    /// </summary>
    public static readonly WarehouseStatus Disabled = new WarehouseStatus(200, "禁用", "禁用");

    /// <summary>
    /// None
    /// </summary>
    public static readonly WarehouseStatus None = new WarehouseStatus(0, "", "");

    private static readonly IReadOnlyList<WarehouseStatus> codeTypeList = new List<WarehouseStatus>() {
        Enabled,
        Disabled,
    };

    private static readonly IDictionary<int, WarehouseStatus> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static WarehouseStatus GetById(int id)
    {
        if (codeTypeMap.ContainsKey(id))
        {
            return codeTypeMap[id];
        }

        return None;
    }

    public static IList<WarehouseStatus> GetList()
    {
        return codeTypeList.ToList();
    }

    private WarehouseStatus(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class WarehouseStatusHibernateType : CodeHibernateTypeBase<WarehouseStatus>
{
    protected override WarehouseStatus GetCodeTypeById(int id)
    {
        return WarehouseStatus.GetById(id);
    }
}

public class WarehouseStatusModelBinder : CodeTypeModelBinderBase<WarehouseStatus>
{
    protected override WarehouseStatus GetCodeTypeById(int id)
    {
        return WarehouseStatus.GetById(id);
    }
}