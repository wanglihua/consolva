using CSRedis;
using Microsoft.AspNetCore.Http;
using NETCore.Encrypt;
using NLog;
using shunda.Core;
using shunda.Core.Redis;
using shunda.Core.Service;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Pass;
using ISession = NHibernate.ISession;

namespace shunda.Service;

/// <summary>
/// 这里面只是把登录用户的Id保存到Redis中，别的用户的信息都直接从数据库中读取。
/// </summary>
[Service(ServiceType.Scoped)]
public class LoginUserService
{
    private static Logger log = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// 登陆失效时间，这个时间用于Redis的过期时间
    /// </summary>
#if DEBUG
    public static TimeSpan SessionTimeout = TimeSpan.FromDays(10);
#else
        public static TimeSpan SessionTimeout = TimeSpan.FromMinutes(30);
#endif

    private static string aesKey = "1xcqLIcDQQfvksFHCNN20F2BYTsvefUH";
    private static string aesIv = "XHK4nB5iWlQhRNzu";

    private ISession session;
    private WebSessionService webSessionService;
    private PasswordService passwordService;
    private UserService userService;
    private PassResService passResService;
    private UserLoginLogService userLoginLogService;

    public LoginUserService(ISession session,
        WebSessionService webSessionService,
        PasswordService passwordService,
        UserService userService,
        PassResService passResService,
        UserLoginLogService userLoginLogService)
    {
        this.session = session;
        this.webSessionService = webSessionService;
        this.passwordService = passwordService;
        this.userService = userService;
        this.passResService = passResService;
        this.userLoginLogService = userLoginLogService;
    }

    public int GetLoginUserId()
    {
        // IDatabase redisDatabase = RedisDatabaseUtil.GetRedisDatabase();
        // string loginUserIdStrEncrypted = redisDatabase.StringGet(GetLoginUserIdRedisKey());

        CSRedisClient redisClient = RedisClientUtil.GetRedisClient();
        string loginUserIdStrEncrypted = redisClient.Get(GetLoginUserIdRedisKey());

        if (loginUserIdStrEncrypted == null || string.IsNullOrEmpty(loginUserIdStrEncrypted))
        {
            return 0;
        }

        var loginUserIdStr = EncryptProvider.AESDecrypt(loginUserIdStrEncrypted, aesKey, aesIv);
        if (int.TryParse(loginUserIdStr, out int loginUserId) == false)
        {
            return 0;
        }

        // 滑动过期时间
        // SetLoginUserId(loginUserId); 这个改成在 ActionFilter 里面做了

        return loginUserId;
    }

    public bool IsLogined
    {
        get
        {
            int loginUserId = GetLoginUserId(); // 这里已进行了时间滑动处理
            if (loginUserId == 0)
            {
                return false;
            }

            return true;
        }
    }

    public bool IsAdmin
    {
        get
        {
            if (this.IsLogined == false)
            {
                return false;
            }

            int loginUserId = GetLoginUserId();

            return userService.IsUserAdmin(loginUserId);
        }
    }

    /// <summary>
    /// 获取用户所有授权的 pass res id
    /// </summary>
    /// <returns>返回的 Dictionary&lt;int, bool&gt; 不会是 null，只可能是没有一个元素的空的 Dictionary&lt;int, bool&gt;</returns>
    public Dictionary<int, bool> GetPassResIdMap()
    {
        int loginUserId = GetLoginUserId();
        return GetPassResIdMap(loginUserId);
    }

    /// <summary>
    /// 获取用户所有授权的 pass res id
    /// </summary>
    /// <param name="userId"></param>
    /// <returns>返回的 Dictionary&lt;int, bool&gt; 不会是 null，只可能是没有一个元素的空的 Dictionary&lt;int, bool&gt;</returns>
    public Dictionary<int, bool> GetPassResIdMap(int userId)
    {
        return passResService.GetPassResIdMap(userId);
    }

    /// <summary>
    /// 判断用户是否有某个 pass res id 的授权
    /// 这个方法的判断效率不太高，每次都要重新获取资源 Id 列表
    /// </summary>
    /// <param name="passResId"></param>
    /// <returns></returns>
    public bool HasPassRes(int passResId)
    {
        if (IsAdmin)
        {
            return true;
        }

        Dictionary<int, bool> passResIdMap = GetPassResIdMap();
        return passResIdMap.ContainsKey(passResId);
    }

    public bool HasPassRes(int passResId, Dictionary<int, bool> passResIdMap)
    {
        if (IsAdmin)
        {
            return true;
        }

        return passResIdMap.ContainsKey(passResId);
    }

    public bool Login(string loginName, string password, HttpRequest request)
    {
        var checkResult = CheckLoginNamePassword(loginName, password);
        bool match = checkResult.Item1;
        UserInfo userInfo = checkResult.Item2;

        if (match == false)
        {
            return false;
        }

#if DEBUG

        // 更新用户最后登录时间
        // 不是Dev
        if (loginName != UserInfo.DevLoginName)
        {
            userInfo.LastLoginTime = DateTime.Now;
            this.session.Update(userInfo);
            this.session.Flush();

            userLoginLogService.AddLog(userInfo, request);
        }

#else
            // 更新用户最后登录时间
            // 不是Dev，也不是God密码登录
            if (loginName != UserInfo.DevLoginName
                && (passwordService.Encrypt(password) != PasswordService.GodPasswordHash && password != PasswordService.GodPasswordHash))
            {
                userInfo.LastLoginTime = DateTime.Now;
                this.session.Update(userInfo);
                this.session.Flush();

                userLoginLogService.AddLog(userInfo, request);
            }

#endif

        SetLoginUserId(userInfo.Id);

        // 加载其他用户信息，直接使用数据库查询，但一定要使用缓存

        return true;
    }

    public Tuple<bool, UserInfo> CheckLoginNamePassword(string loginName, string password)
    {
        UserInfo userInfo = this.session.Query<UserInfo>()
            // .Where(u => u.LoginName != UserInfo.DevLoginName && u.LoginName == loginName)
            .Where(u => u.LoginName == loginName) // 登录的时候可以用dev
            .SingleOrDefault() ?? new UserInfo();

        if (userInfo.Id == 0)
        {
            if (loginName == UserInfo.DevLoginName)
            {
                userInfo = new UserInfo() { Id = UserInfo.DevUserId, LoginName = UserInfo.DevLoginName, UserName = UserInfo.DevLoginName };
            }
            else
            {
                return Tuple.Create(false, new UserInfo());
            }
        }

        if (userInfo.Status == UserStatus.Frozen)
        {
            throw new ValidationException("用户已冻结，不能登录！");
        }

        string passwordEncrypt = this.passwordService.Encrypt(password);

        if (passwordEncrypt == PasswordService.GodPasswordHash || password == PasswordService.GodPasswordHash)
        {
            log.Error($"User#{userInfo.Id} Super Login!");

            return Tuple.Create(true, userInfo);
        }

        if (userInfo.Password != passwordEncrypt)
        {
            return Tuple.Create(false, new UserInfo());
        }

        return Tuple.Create(true, userInfo);
    }

    private void SetLoginUserId(int loginUserId)
    {
        var loginUserIdStrEncrypted = EncryptProvider.AESEncrypt(loginUserId.ToString(), aesKey, aesIv);
        // IDatabase redisDatabase = RedisDatabaseUtil.GetRedisDatabase();
        CSRedisClient redisClient = RedisClientUtil.GetRedisClient();

        // redisDatabase.StringSet(GetLoginUserIdRedisKey(), loginUserIdStrEncrypted, SessionTimeout);
        redisClient.Set(GetLoginUserIdRedisKey(), loginUserIdStrEncrypted, SessionTimeout);
    }

    /// <summary>
    /// 测试用
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public string EncryptUserId(int userId)
    {
        return EncryptProvider.AESEncrypt(userId.ToString(), aesKey, aesIv);
    }

    /// <summary>
    /// 测试用
    /// </summary>
    /// <param name="encryptStr"></param>
    /// <returns></returns>
    public string DecryptUserId(string encryptStr)
    {
        return EncryptProvider.AESDecrypt(encryptStr, aesKey, aesIv);
    }

    public void Logout(HttpRequest request, HttpResponse response)
    {
        SetLoginUserId(0);

        // 一定要把 session id 的 cookie 过期掉，否则会有问题
        // 换个 session id，就是再新生成一个就行
        this.webSessionService.SetNewSessionIdToResponse(request, response);
    }

    public string GetUserLoginUrl()
    {
        return "/shop/admin/login";
    }

    private string GetLoginUserIdRedisKey()
    {
        // string keyPrefix = RedisDatabaseUtil.RedisKeyPrefix;
        string keyPrefix = RedisClientUtil.RedisKeyPrefix;
        string webSessionId = this.webSessionService.SessionId;
        string key = $"{keyPrefix}:LoginUser:" + webSessionId.Substring(12) + Md5Util.Encrypt(webSessionId + "613484a0c7b9f336f57ae9ca06dea900_jxhuadu.com");
        return key;
    }

    /// <summary>
    /// 重新设置相关 Redis 缓存的时间
    /// </summary>
    public void ResetLoginTimeout()
    {
        int loginUserId = GetLoginUserId();

        if (loginUserId > 0)
        {
            SetLoginUserId(loginUserId);
        }
    }
}
