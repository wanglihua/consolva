using Microsoft.Extensions.Caching.Memory;
using NHibernate;
using NLog;
using shunda.Core.Service;
using shunda.Domain.Pass;

namespace shunda.Service.Pass;

[Service(ServiceType.Scoped)]
public class AdminUserService {
    private static Logger log = LogManager.GetCurrentClassLogger();

    private static readonly string AdminUserIdListCacheKey = "admin_user_id_map_all";
    private static readonly TimeSpan AdminUserIdListCacheTimeSpan = TimeSpan.FromHours(1); // 这个缓存时间是否合理

    private readonly ISession session;
    private readonly IMemoryCache memoryCache;

    public AdminUserService(ISession session, IMemoryCache memoryCache) {
        this.session = session;
        this.memoryCache = memoryCache;
    }

    public bool IsUserAdmin(int userId) {
        if (userId == UserInfo.DevUserId) {
            return true;
        }

        Dictionary<int, bool> adminUserIdDict = RetriveOrGetCachedAdminUserIdMap();

        if (adminUserIdDict.TryGetValue(userId, out bool admin) == true) {
            return admin; // 一般都为 true
        }

        return false;
    }

    public Dictionary<int, bool> RetriveOrGetCachedAdminUserIdMap() {
        if (memoryCache.TryGetValue(AdminUserIdListCacheKey, out Dictionary<int, bool> adminUserIdDict) == false) {
            log.Trace("admin_user_id_map load from cache failed");

            adminUserIdDict = RetriveAdminUserIdMap();
            memoryCache.Set(AdminUserIdListCacheKey, adminUserIdDict, AdminUserIdListCacheTimeSpan);
        } else {
            log.Trace("admin_user_id_map load from cache success");
        }

        return adminUserIdDict;
    }

    public Dictionary<int, bool> RetriveAdminUserIdMap() {
        List<int> userIdList = session.Query<AdminInfo>()
            .Select(e => e.User.Id)
            .ToList();

        Dictionary<int, bool> adminUserIdDict = new Dictionary<int, bool>();

        foreach (int userId in userIdList) {
            adminUserIdDict[userId] = true;
        }

        return adminUserIdDict;
    }

    public void CacheAdminUserIdMap() {
        Dictionary<int, bool> adminUserIdDict = RetriveAdminUserIdMap();
        memoryCache.Set(AdminUserIdListCacheKey, adminUserIdDict, AdminUserIdListCacheTimeSpan);
    }
}