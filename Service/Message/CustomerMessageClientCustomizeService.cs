using Microsoft.Extensions.Caching.Memory;
using NHibernate;
using shunda.Core.Service;
using shunda.Domain.Message;

namespace shunda.Service.Message;

/// <summary>
/// 客户消息定客户端制
/// todo d 改成redis缓存
/// </summary>
[Service(ServiceType.Scoped)]
public class CustomerMessageClientCustomizeService {
    private readonly ISession session;
    private readonly IMemoryCache memoryCache;

    private readonly TimeSpan CACHE_TIME_SPAN = TimeSpan.FromMinutes(10);

    public CustomerMessageClientCustomizeService(ISession session, IMemoryCache memoryCache) {
        this.session = session;
        this.memoryCache = memoryCache;
    }

    private string GetCacheKey(int customerId, CustomerMessageType messageType) {
        return $"{customerId}_{messageType.Id}";
    }

    /// <summary>
    /// 一般是后台逻辑发送消息时会调用这个
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="messageType"></param>
    /// <returns></returns>
    public bool ShouldSend(int customerId, CustomerMessageType messageType) {
        string cacheKey = GetCacheKey(customerId, messageType);

        if (memoryCache.TryGetValue(cacheKey, out bool shouldSend)) {
            return shouldSend;
        }

        var clientCustomize = session.Query<CustomerMessageClientCustomizeInfo>()
            .Where(e => e.CustomerId == customerId && e.MessageType == messageType)
            .Take(1)
            .SingleOrDefault();

        if (clientCustomize == null) {
            shouldSend = true;
        } else {
            shouldSend = clientCustomize.Send;
        }

        memoryCache.Set(cacheKey, shouldSend, CACHE_TIME_SPAN);

        return shouldSend;
    }

    /// <summary>
    /// 客户端保存设置时调用这个
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="messageType"></param>
    /// <param name="shouldSend"></param>
    /// <returns>生效时间，单位分钟</returns>
    public int Save(int customerId, CustomerMessageType messageType, bool shouldSend) {
        var clientCustomize = session.Query<CustomerMessageClientCustomizeInfo>()
            .Where(e => e.CustomerId == customerId && e.MessageType == messageType)
            .Take(1)
            .SingleOrDefault();
        if (clientCustomize == null) {
            clientCustomize = new CustomerMessageClientCustomizeInfo() {
                CustomerId = customerId,
                MessageType = messageType,
            };
        }

        clientCustomize.Send = shouldSend;

        session.SaveOrUpdate(clientCustomize);
        session.Flush();

        string cacheKey = GetCacheKey(customerId, messageType);
        memoryCache.Set(cacheKey, shouldSend, CACHE_TIME_SPAN);

        return (int)(CACHE_TIME_SPAN.TotalMinutes);
    }
}