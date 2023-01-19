using Microsoft.Extensions.Caching.Memory;
using shunda.Core;
using shunda.Core.Service;

namespace shunda.Admin.Services;

/// <summary>
/// 后台用户登录频率限制，不能过于频繁
/// 主要是限制APP端
/// </summary>
[Service(ServiceType.Scoped)]
public class UserLoginRateLimitService {
    private readonly IMemoryCache memoryCache;

    public UserLoginRateLimitService(IMemoryCache memoryCache) {
        this.memoryCache = memoryCache;
    }

    public void CheckRateLimit(string loginName) {
        if (string.IsNullOrWhiteSpace(loginName)) {
            throw new ValidationException("登录名不能为空！");
        }

        string key = "user_login_rate_count_" + loginName;
        if (memoryCache.TryGetValue(key, out IList<DateTime> loginTimeList) == false) {
            loginTimeList = new List<DateTime>();
        }

        // 检查值
        DateTime now = DateTime.Now;
        int loginCount = loginTimeList.Where(e => e >= now.AddSeconds(-60)).Count(); // 最近有缓存记录的登录次数
        // 60 秒内最多登录次数不能大于 5 次
        if (loginCount > 5) {
            throw new ValidationException("当前用户登录过于频繁！");
        }

        // 存储当前尝试登录时间
        loginTimeList.Add(now);

        memoryCache.Set(key, loginTimeList, TimeSpan.FromMinutes(10)); // 缓存保留10分钟
    }
}