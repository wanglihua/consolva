using Microsoft.Extensions.Caching.Memory;
using NHibernate;
using NLog;
using shunda.Core.Service;
using shunda.Core.Util;
using shunda.Domain.Pass;

namespace shunda.Service.Pass;

[Service(ServiceType.Scoped)]
public class PassResService {
    private static Logger log = LogManager.GetCurrentClassLogger();

    private static readonly string PassResCachekey = "user_pass_res_dict_for_all";
    private static readonly TimeSpan PassResCacheTimeSpan = TimeSpan.FromHours(1); // 这个缓存时间是否合理

    private readonly ISession session;
    private readonly IMemoryCache memoryCache;
    private readonly UserService userService;

    public PassResService(ISession session, IMemoryCache memoryCache, UserService userService) {
        this.session = session;
        this.memoryCache = memoryCache;
        this.userService = userService;
    }

    /// <summary>
    /// 用户是否有对应的权限
    /// 这个方法的判断是使用缓存数据的
    /// </summary>
    public bool UserHasPassRes(int userId, int passResId) {
        bool isAdmin = userService.IsUserAdmin(userId);
        Dictionary<int, bool> passResIdMap = GetPassResIdMap(userId);

        return PassUtil.HasPassRes(passResIdMap, isAdmin, passResId);
    }

    /// <summary>
    /// 用户是否有对应的权限
    /// 这个方法的判断是使用缓存数据的
    /// </summary>
    public bool UserHasOneOfPassRes(int userId, params int[] passResIds) {
        bool isAdmin = userService.IsUserAdmin(userId);
        Dictionary<int, bool> passResIdMap = GetPassResIdMap(userId);
        return PassUtil.HasOneOfPassRes(passResIdMap, isAdmin, passResIds);
    }

    /// <summary>
    /// 获取用户所有授权的 pass res id
    /// </summary>
    /// <param name="userId"></param>
    /// <returns>返回的 Dictionary&lt;int, bool&gt; 不会是 null，只可能是没有一个元素的空的 Dictionary&lt;int, bool&gt;</returns>
    public Dictionary<int, bool> GetPassResIdMap(int userId) {
        Dictionary<int, Dictionary<int, bool>> userPassResDict = RetriveOrGetCachedPassResIdMapForAllUser();
        if (userPassResDict.TryGetValue(userId, out Dictionary<int, bool>? passResDict) == false) {
            passResDict = new Dictionary<int, bool>();
        }

        return passResDict;
    }

    public Dictionary<int, Dictionary<int, bool>> RetriveOrGetCachedPassResIdMapForAllUser() {
        if (memoryCache.TryGetValue(PassResCachekey, out Dictionary<int, Dictionary<int, bool>> userPassResDict) == false) {
            log.Trace("user_pass_res_dict load from cache failed");

            userPassResDict = RetrivePassResIdMapForAllUser();
            memoryCache.Set(PassResCachekey, userPassResDict, PassResCacheTimeSpan);
        } else {
            log.Trace("user_pass_res_dict load from cache success");
        }

        return userPassResDict;
    }

    public void CachePassResIdMapForAllUser() {
        Dictionary<int, Dictionary<int, bool>> userPassResDict = RetrivePassResIdMapForAllUser();
        memoryCache.Set(PassResCachekey, userPassResDict, PassResCacheTimeSpan);
    }

    public Dictionary<int, Dictionary<int, bool>> RetrivePassResIdMapForAllUser() {
        var userPassResList = session.Query<UserPassResInfo>()
            .Select(e => new {
                UserId = e.User.Id,
                PassResId = e.PassRes.Id
            })
            .ToList();

        var groupUserPassResList = session.Query<GroupPassResInfo>()
            .Join(session.Query<GroupMemberInfo>(),
                gpr => gpr.Group.Id,
                gm => gm.Group.Id,
                (gpr, gm) => new { gpr, gm })
            .Join(session.Query<GroupInfo>(),
                e => e.gm.Group.Id,
                g => g.Id,
                (e, g) => new { e.gpr, e.gm, g })
            .Select(e => new {
                UserId = e.gm.User.Id,
                PassResId = e.gpr.PassRes.Id
            })
            .ToList();

        Dictionary<int, Dictionary<int, bool>> userPassResDict = new Dictionary<int, Dictionary<int, bool>>();

        // 直接设置到用户的权限
        foreach (var upr in userPassResList) {
            if (userPassResDict.TryGetValue(upr.UserId, out Dictionary<int, bool>? passResDict) == false) {
                passResDict = new Dictionary<int, bool>();
            }

            passResDict[upr.PassResId] = true;

            userPassResDict[upr.UserId] = passResDict;
        }

        // 通过群组设置的权限
        foreach (var upr in groupUserPassResList) {
            if (userPassResDict.TryGetValue(upr.UserId, out Dictionary<int, bool>? passResDict) == false) {
                passResDict = new Dictionary<int, bool>();
            }

            passResDict[upr.PassResId] = true;

            userPassResDict[upr.UserId] = passResDict;
        }

        return userPassResDict;
    }
}