using NHibernate;
using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.Logistics;
using shunda.Domain.Pass;

namespace shunda.Service.Logistics;

[Service(ServiceType.Scoped)]
public class LogisticsGoodsLogService
{
    private ISession session;

    public LogisticsGoodsLogService(ISession session)
    {
        this.session = session;
    }

    public void AddLog(LogisticsGoodsLogType type, int goodsId, int userId)
    {
        AddLog(type, goodsId, userId, string.Empty);
    }

    /// <summary>
    /// 需要结束后外部调用 session.Flush();
    /// </summary>
    /// <param name="type"></param>
    /// <param name="goodsId"></param>
    /// <param name="userId"></param>
    /// <param name="description"></param>
    /// <exception cref="ValidationException"></exception>
    public void AddLog(LogisticsGoodsLogType type, int goodsId, int userId, string description)
    {
        type ??= LogisticsGoodsLogType.None;

        if (string.IsNullOrWhiteSpace(description))
        {
            description = string.Empty;
        }

        LogisticsGoodsInfo? goods = session.Get<LogisticsGoodsInfo>(goodsId);
        if (goods == null)
        {
            throw new ValidationException($"指定的物品#{goodsId}不存在！");
        }

        UserInfo? user = session.Get<UserInfo>(userId);
        if (user == null)
        {
            throw new ValidationException($"指定的用户#{userId}不存在！");
        }

        LogisticsGoodsLogInfo log = new LogisticsGoodsLogInfo();
        log.Type = type;
        log.Goods = goods;
        log.User = user;
        log.Description = description;

        session.Save(log);
        session.Flush();
    }
}