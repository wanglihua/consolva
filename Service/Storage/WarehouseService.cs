using Microsoft.Extensions.Caching.Memory;
using shunda.Core.Service;
using shunda.Domain.Storage;
using ISession = NHibernate.ISession;

namespace shunda.Service.Storage;

[Service(ServiceType.Scoped)]
public class WarehouseService {
    private readonly ISession session;
    private readonly IMemoryCache memoryCache;

    public WarehouseService(ISession session, IMemoryCache memoryCache) {
        this.session = session;
        this.memoryCache = memoryCache;
    }

    public IList<WarehouseInfo> GetWarehouseListForAdmin() {
        return session.Query<WarehouseInfo>()
            .Where(e => e.InnerStatus == WarehouseStatus.Enabled)
            .OrderBy(e => e.OrderNum)
            .ThenBy(e => e.Id)
            .ToList();
    }

    private const string WAREHOUSE_LIST_CACHE_KEY = "warehouse_list_cache_key";

    /// <summary>
    /// 获取仓库列表，优先从缓存中获取
    /// </summary>
    /// <returns></returns>
    public IList<WarehouseInfo> GetWarehouseListWithCacheForAdmin() {
        if (memoryCache.TryGetValue(WAREHOUSE_LIST_CACHE_KEY, out IList<WarehouseInfo>? warehouseList) == false) {
            warehouseList = GetWarehouseListForAdmin();

            memoryCache.Set(WAREHOUSE_LIST_CACHE_KEY, warehouseList, TimeSpan.FromHours(8));
        }

        return warehouseList!;
    }

    /// <summary>
    /// 使仓库列表缓存失效
    /// </summary>
    public void InvalidWarehouseListCache() {
        memoryCache.Remove(WAREHOUSE_LIST_CACHE_KEY);
    }
}