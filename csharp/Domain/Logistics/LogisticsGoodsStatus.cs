using shunda.Core.CodeType;

namespace shunda.Domain.Logistics;

public class LogisticsGoodsStatus : CodeTypeBase
{
    /// <summary>
    /// 需拍照
    /// </summary>
    public static readonly LogisticsGoodsStatus PhotoRequired = new LogisticsGoodsStatus(50, "需拍照", "需拍照");

    /// <summary>
    /// 未确认
    /// </summary>
    public static readonly LogisticsGoodsStatus NotConfirmed = new LogisticsGoodsStatus(100, "未确认", "未确认");

    /// <summary>
    /// 已确认
    /// </summary>
    public static readonly LogisticsGoodsStatus Confirmed = new LogisticsGoodsStatus(200, "已确认", "已确认");

    /// <summary>
    /// 已禁用
    /// </summary>
    public static readonly LogisticsGoodsStatus Disabled = new LogisticsGoodsStatus(300, "已禁用", "已禁用");

    /// <summary>
    /// None
    /// </summary>
    public static readonly LogisticsGoodsStatus None = new LogisticsGoodsStatus(0, "", "");

    public static LogisticsGoodsStatus GetById(int id)
    {
        IList<LogisticsGoodsStatus> goodsStatusList = GetList();
        foreach (LogisticsGoodsStatus goodsStatusInfo in goodsStatusList)
        {
            if (goodsStatusInfo.Id == id)
            {
                return goodsStatusInfo;
            }
        }

        return None;
    }

    /// <summary>
    /// 获取列表
    /// </summary>
    /// <returns></returns>
    public static IList<LogisticsGoodsStatus> GetList()
    {
        IList<LogisticsGoodsStatus> goodsStatusList = new List<LogisticsGoodsStatus>();
        goodsStatusList.Add(PhotoRequired);
        goodsStatusList.Add(NotConfirmed);
        goodsStatusList.Add(Confirmed);
        goodsStatusList.Add(Disabled);

        return goodsStatusList;
    }

    private LogisticsGoodsStatus(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class LogisticsGoodsStatusHibernateType : CodeHibernateTypeBase<LogisticsGoodsStatus>
{
    protected override LogisticsGoodsStatus GetCodeTypeById(int id)
    {
        return LogisticsGoodsStatus.GetById(id);
    }
}

public class LogisticsGoodsStatusModelBinder : CodeTypeModelBinderBase<LogisticsGoodsStatus>
{
    protected override LogisticsGoodsStatus GetCodeTypeById(int id)
    {
        return LogisticsGoodsStatus.GetById(id);
    }
}