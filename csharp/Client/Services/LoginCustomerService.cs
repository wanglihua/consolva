using CSRedis;
using NETCore.Encrypt;
using NHibernate.Linq;
using NLog;
using shunda.Core.Redis;
using shunda.Core.Service;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Customer;
using shunda.Service.Customer;
using shunda.Service.Pass;
using shunda.Service.Wechat;
using ISession = NHibernate.ISession;

namespace shunda.Client.Services;

/// <summary>
/// 这里面只是把登录用户的Id保存到Redis中，别的用户的信息都直接从数据库中读取，就是注意数据库查询的时候要缓存。
/// </summary>
[Service(ServiceType.Scoped)]
public class LoginCustomerService
{
    private static Logger log = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// 登陆失效时间，这个时间用于Redis的过期时间，公众号登陆时，不用这个时间，公众号登陆应该是长期可以的
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
    private CustomerSupperPasswordService customerSupperPasswordService;
    private CustomerCodeService customerCodeService;
    private WechatSessionService wechatSessionService;
    private CustomerLoginLogService customerLoginLogService;

    public LoginCustomerService(ISession session,
        WebSessionService webSessionService,
        PasswordService passwordService,
        CustomerSupperPasswordService customerSupperPasswordService,
        CustomerCodeService customerCodeService,
        WechatSessionService wechatSessionService,
        CustomerLoginLogService customerLoginLogService)
    {
        this.session = session;
        this.webSessionService = webSessionService;
        this.passwordService = passwordService;
        this.customerSupperPasswordService = customerSupperPasswordService;
        this.customerCodeService = customerCodeService;
        this.wechatSessionService = wechatSessionService;
        this.customerLoginLogService = customerLoginLogService;
    }

    public int GetLoginCustomerId()
    {
        CSRedisClient redisClient = RedisClientUtil.GetRedisClient();
        string loginCustomerIdStrEncrypted = redisClient.Get(GetLoginCustomerIdRedisKey());

        if (string.IsNullOrWhiteSpace(loginCustomerIdStrEncrypted))
        {
            return 0;
        }

        var loginCustomerIdStr = EncryptProvider.AESDecrypt(loginCustomerIdStrEncrypted, aesKey, aesIv);
        if (int.TryParse(loginCustomerIdStr, out int loginCustomerId) == false)
        {
            return 0;
        }

        // 滑动过期时间
        SetLoginCustomerId(loginCustomerId); // 这个已经在 ActionFilter 里面做了，这里再做一下也可以的

        return loginCustomerId;
    }

    public bool IsLogined
    {
        get
        {
            int loginCustomerId = GetLoginCustomerId();
            if (loginCustomerId == 0)
            {
                return false;
            }

            return true;
        }
    }

    public string GetCustomerLoginUrl()
    {
        return "/client/login";
    }

    /// <summary>
    /// 使用用户名和密码登录
    /// </summary>
    /// <param name="loginName"></param>
    /// <param name="password"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    public LoginResult Login(string loginName, string password, HttpRequest request)
    {
        if (string.IsNullOrWhiteSpace(loginName)) // 手机号默认为空，必须防止空的手机号的登录
        {
            return new LoginResult(LoginResultCode.AccountNotExist, null);
        }

        int customerId = 0;

        CustomerInfo? customer = null;

        // 这是一个事务，或者没应用事务
        {
            // 先尝试使用 客户ID登录
            int customerIdFromInput = customerCodeService.GetCustomerIdFromCustomerCode(loginName);
            if (customerIdFromInput != 0)
            {
                customer = session.Get<CustomerInfo>(customerIdFromInput);
            }

            if (customer == null)
            {
                customer = session.Query<CustomerInfo>()
                    .Where(u => (u.Mobile == loginName && u.MobileValidated == true) || u.CheckinCode == loginName) // 验证过的手机号，入库码 可以登录
                    .Take(1)
                    .SingleOrDefault();
            }

            if (customer == null)
            {
                return new LoginResult(LoginResultCode.AccountNotExist, null);
            }

            if (customer.Status.Id == CustomerStatus.Frozen.Id)
            {
                return new LoginResult(LoginResultCode.CustomerFrozen, null);
            }

            if (passwordService.Encrypt(password) == PasswordService.GodPasswordHash || password == PasswordService.GodPasswordHash)
            {
                log.Warn("Customer Super Login!");

                // 这里的逻辑本来 LoginInternal 里面就会做的, 但 God Password 时不调用 LoginInternal
                SetLoginCustomerId(customer.Id);

                return new LoginResult(LoginResultCode.Success, customer);
            }

            string salt = customer.Salt;
            string passwordEncrypt = passwordService.Encrypt(password, salt);
            if (customer.Password != passwordEncrypt && customerSupperPasswordService.CheckPasswordValid(customer.Id, password) == false) // 检查普通密码，同时 尝试超级密码
            {
                // 普通密码 和 超级密码 都不行
                return new LoginResult(LoginResultCode.PasswordNotMatch, null);
            }

            customerId = customer.Id;
        }

        {
            // 是否是微信环境访问，如果是将当前 open id 保存到用户信息里面去
            if (WebUtil.IsWeiXinRequest(request))
            {
                // 这是一个事务
                wechatSessionService.BindOpenIdToCustomerAndValidatePromotionNewThread(customerId);
            }
        }

        LoginInternal(customerId, request);

        // 客户端登录日志
        customerLoginLogService.AddLog(CustomerLoginStage.WebManualLogin, customerId, request);

        return new LoginResult(LoginResultCode.Success, customer);
    }

    /// <summary>
    /// 使用 open id 登录
    /// </summary>
    /// <param name="openId"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    public LoginResult Login(string openId, HttpRequest request)
    {
        int customerId = 0;

        // 这是一个事务，或者没应用事务

        CustomerInfo? customer = session.Query<CustomerInfo>().Where(e => e.OpenId == openId).Take(1).SingleOrDefault();
        if (customer == null)
        {
            return new LoginResult(LoginResultCode.AccountNotExist, null);
        }

        if (customer.Status == CustomerStatus.Frozen)
        {
            return new LoginResult(LoginResultCode.CustomerFrozen, null);
        }

        customerId = customer.Id;

        LoginInternal(customerId, request);

        // 客户端登录日志
        customerLoginLogService.AddLog(CustomerLoginStage.WechatOfficalAccountLogin, customerId, request);

        return new LoginResult(LoginResultCode.Success, customer);
    }

    /// <summary>
    /// 更新客户数据库登录信息，设置登录上下文
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    public void LoginInternal(int customerId, HttpRequest request)
    {
        SetLoginCustomerId(customerId);

        string clientIp = WebUtil.GetClientIp(request);
        DateTime now = DateTime.Now;
        session.Query<CustomerInfo>()
            .Where(e => e.Id == customerId)
            .UpdateBuilder()
            .Set(e => e.LastLogin, now)
            .Set(e => e.UpdateTime, now)
            .Set(e => e.LastIp, clientIp)
            .Update();
    }

    private void SetLoginCustomerId(int loginCustomerId)
    {
        var loginCustomerIdStrEncrypted = EncryptProvider.AESEncrypt(loginCustomerId.ToString(), aesKey, aesIv);

        CSRedisClient redisClient = RedisClientUtil.GetRedisClient();
        redisClient.Set(GetLoginCustomerIdRedisKey(), loginCustomerIdStrEncrypted, SessionTimeout);
    }

    public void Logout(HttpRequest request, HttpResponse response)
    {
        SetLoginCustomerId(0);

        // 一定要把 session id 的 cookie 过期掉，否则会有问题
        // 换个 session id，就是再新生成一个就行
        webSessionService.PutNewSessionIdToResponse(request, response);
    }

    private string GetLoginCustomerIdRedisKey()
    {
        string keyPrefix = RedisClientUtil.RedisKeyPrefix;
        string webSessionId = webSessionService.SessionId;

        string key = $"{keyPrefix}:customer:" + webSessionId.Substring(0, 12) + Md5Util.Encrypt(webSessionId + "613484a0c7b9f336f57ae9ca06dea900_jxhuadu.com");
        return key;
    }

    /// <summary>
    /// 重新设置相关 Redis 缓存的时间
    /// </summary>
    public void ResetLoginTimeout()
    {
        int loginCustomerId = GetLoginCustomerId();

        if (loginCustomerId > 0)
        {
            SetLoginCustomerId(loginCustomerId);
        }

        // openid 也一定要滑动，否则 jsapi支付 会出错
        string openId = wechatSessionService.GetOpenId();
        if (string.IsNullOrWhiteSpace(openId) == false)
        {
            wechatSessionService.SetOpenId(openId);
        }
    }
}

public class LoginResult
{
    public LoginResult(LoginResultCode resultCode, CustomerInfo? customer)
    {
        this.ResultCode = resultCode;
        this.Customer = customer;
    }

    public LoginResultCode ResultCode { get; set; }

    public CustomerInfo? Customer { get; set; }
}

public enum LoginResultCode
{
    Success,
    AccountNotExist,
    PasswordNotMatch,
    CustomerFrozen,
}