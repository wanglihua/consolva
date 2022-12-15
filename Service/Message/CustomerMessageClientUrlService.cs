using Microsoft.Extensions.Caching.Memory;
using shunda.Core.Service;
using shunda.Domain;
using shunda.Domain.Message;
using ISession = NHibernate.ISession;

namespace shunda.Service.Message;

/// <summary>
/// 客户消息客户端URL 服务类
/// </summary>
[Service(ServiceType.Scoped)]
public class CustomerMessageClientUrlService
{
    private readonly ISession session;
    private IMemoryCache memoryCache;

    public CustomerMessageClientUrlService(ISession session, IMemoryCache memoryCache)
    {
        this.session = session;
        this.memoryCache = memoryCache;
    }

    private static string CACHE_KEY = "CUSTOMER_MESSAGE_CLIENT_URL";

    public string GetUrl(TerminalType terminalType, CustomerMessageType messageType)
    {
        if (memoryCache.TryGetValue(CACHE_KEY, out IList<CustomerMessageClientUrlInfo> messageUrlList) == false)
        {
            messageUrlList = session.Query<CustomerMessageClientUrlInfo>().ToList();
            memoryCache.Set(CACHE_KEY, messageUrlList, TimeSpan.FromMinutes(5)); // 5 分钟是否合适，消息频繁发送的时候会有收益
        }

        var messageUrl = messageUrlList
            .Where(e => e.TerminalType == terminalType && e.MessageType == messageType)
            .Take(1)
            .SingleOrDefault();

        if (messageUrl == null)
        {
            return string.Empty;
        }

        return messageUrl.ClientUrl;
    }

    /// <summary>
    /// 使缓存失效，这个只能对当前应用实例有效。
    /// 可能不同的功能部分，会被部署成不同的应用实例。
    /// </summary>
    public void MakeCacheExpire()
    {
        memoryCache.Remove(CACHE_KEY);
    }

    // todo 加上可能的模板参数替换方式的URL类型
}