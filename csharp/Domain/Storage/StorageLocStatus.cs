using shunda.Core.CodeType;

namespace shunda.Domain.Storage;

/// <summary>
/// 库位状态
/// </summary>
public class StorageLocStatus : CodeTypeBase
{
    /// <summary>
    /// 启用，1000
    /// </summary>
    public static readonly StorageLocStatus Enabled = new StorageLocStatus(1000, "启用", "启用");

    /// <summary>
    /// 停用，2000
    /// </summary>
    public static readonly StorageLocStatus Disabled = new StorageLocStatus(2000, "停用", "停用");

    /// <summary>
    /// None
    /// </summary>
    public static readonly StorageLocStatus None = new StorageLocStatus(0, "", "");

    private static readonly IList<StorageLocStatus> codeTypeList = new List<StorageLocStatus>() {
        Enabled,
        Disabled,
    };

    private static readonly Dictionary<int, StorageLocStatus> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static StorageLocStatus GetById(int id)
    {
        if (codeTypeMap.TryGetValue(id, out StorageLocStatus? codeType))
        {
            return codeType;
        }

        return None;
    }

    public static IList<StorageLocStatus> GetList()
    {
        return codeTypeList;
    }

    private StorageLocStatus(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class StorageLocStatusHibernateType : CodeHibernateTypeBase<StorageLocStatus>
{
    protected override StorageLocStatus GetCodeTypeById(int id)
    {
        return StorageLocStatus.GetById(id);
    }
}

public class StorageLocStatusModelBinder : CodeTypeModelBinderBase<StorageLocStatus>
{
    protected override StorageLocStatus GetCodeTypeById(int id)
    {
        return StorageLocStatus.GetById(id);
    }
}