using shunda.Core.CodeType;

namespace shunda.Domain.Logistics;

/// <summary>
/// 物品操作日志类型
/// </summary>
public class LogisticsGoodsLogType : CodeTypeBase
{
    /// <summary>
    /// 添加
    /// </summary>
    public static readonly LogisticsGoodsLogType Create = new LogisticsGoodsLogType(100, "添加", "添加");


    /// <summary>
    /// 修改
    /// </summary>
    public static readonly LogisticsGoodsLogType Modify = new LogisticsGoodsLogType(200, "修改", "修改");


    /// <summary>
    /// 确认
    /// </summary>
    public static readonly LogisticsGoodsLogType Confirm = new LogisticsGoodsLogType(300, "确认", "确认");

    /// <summary>
    /// 启用
    /// </summary>
    public static readonly LogisticsGoodsLogType Enable = new LogisticsGoodsLogType(400, "启用", "启用");

    /// <summary>
    /// 禁用
    /// </summary>
    public static readonly LogisticsGoodsLogType Disable = new LogisticsGoodsLogType(500, "禁用", "禁用");

    /// <summary>
    /// 设置需拍照
    /// </summary>
    public static readonly LogisticsGoodsLogType SetPhotoRequire = new LogisticsGoodsLogType(600, "设置需拍照", "设置需拍照");

    /// <summary>
    /// None
    /// </summary>
    public static readonly LogisticsGoodsLogType None = new LogisticsGoodsLogType(0, "", "");

    private static readonly IReadOnlyList<LogisticsGoodsLogType> codeTypeList = new List<LogisticsGoodsLogType>() {
        Create,
        Modify,
        Confirm,
        Enable,
        Disable,
        SetPhotoRequire,
    };

    private static readonly Dictionary<int, LogisticsGoodsLogType> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static LogisticsGoodsLogType GetById(int id)
    {
        if (codeTypeMap.TryGetValue(id, out LogisticsGoodsLogType? codeType))
        {
            return codeType;
        }

        return None;
    }

    public static IList<LogisticsGoodsLogType> GetList()
    {
        return codeTypeList.ToList();
    }

    private LogisticsGoodsLogType(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class LogisticsGoodsLogTypeHibernateType : CodeHibernateTypeBase<LogisticsGoodsLogType>
{
    protected override LogisticsGoodsLogType GetCodeTypeById(int id)
    {
        return LogisticsGoodsLogType.GetById(id);
    }
}

public class LogisticsGoodsLogTypeModelBinder : CodeTypeModelBinderBase<LogisticsGoodsLogType>
{
    protected override LogisticsGoodsLogType GetCodeTypeById(int id)
    {
        return LogisticsGoodsLogType.GetById(id);
    }
}