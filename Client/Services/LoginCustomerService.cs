using CSRedis;
using NETCore.Encrypt;
using NHibernate.Linq;
using NLog;
using shunda.Core;
using shunda.Core.Config;
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
public class LoginCustomerService {
    private static Logger log = LogManager.GetCurrentClassLogger();

    private static string AES_KEY => "1xcqLIcDQQfvksFHCNN20F2BYTsvefUH";
    private static string AES_IV => "XHK4nB5iWlQhRNzu";

    private readonly ISession session;
    private readonly WebSessionService webSessionService;
    private readonly PasswordService passwordService;
    private readonly CustomerSupperPasswordService customerSupperPasswordService;
    private readonly CustomerCodeService customerCodeService;
    private readonly WechatOfficialAccountSessionService wechatOfficialAccountSessionService;
    private readonly CustomerLoginLogService customerLoginLogService;

    public LoginCustomerService(ISession session,
        WebSessionService webSessionService,
        PasswordService passwordService,
        CustomerSupperPasswordService customerSupperPasswordService,
        CustomerCodeService customerCodeService,
        WechatOfficialAccountSessionService wechatOfficialAccountSessionService,
        CustomerLoginLogService customerLoginLogService) {
        this.session = session;
        this.webSessionService = webSessionService;
        this.passwordService = passwordService;
        this.customerSupperPasswordService = customerSupperPasswordService;
        this.customerCodeService = customerCodeService;
        this.wechatOfficialAccountSessionService = wechatOfficialAccountSessionService;
        this.customerLoginLogService = customerLoginLogService;
    }

    public int GetLoginCustomerId() {
        CSRedisClient redisClient = RedisClientUtil.GetRedisClient();
        string loginCustomerIdStrEncrypted = redisClient.Get(GetLoginCustomerIdRedisKey());

        if (string.IsNullOrWhiteSpace(loginCustomerIdStrEncrypted)) {
            return 0;
        }

        var loginCustomerIdStr = EncryptProvider.AESDecrypt(loginCustomerIdStrEncrypted, AES_KEY, AES_IV);
        if (int.TryParse(loginCustomerIdStr, out int loginCustomerId) == false) {
            return 0;
        }

        // 滑动过期时间
        SetLoginCustomerId(loginCustomerId); // 这个已经在 ActionFilter 里面做了，这里再做一下也可以的

        return loginCustomerId;
    }

    public CustomerInfo GetLoginCustomer() {
        int customerId = GetLoginCustomerId();
        var customer = session.Get<CustomerInfo>(customerId);
        if (customer == null) {
            throw new ValidationException($"指定的登录客户#{customerId}不存在！");
        }

        return customer;
    }

    public bool IsLogined {
        get {
            int loginCustomerId = GetLoginCustomerId();
            if (loginCustomerId == 0) {
                return false;
            }

            return true;
        }
    }

    public string GetCustomerLoginUrl() {
        return "/client/login";
    }

    /// <summary>
    /// 使用用户名和密码登录
    /// </summary>
    /// <param name="loginName"></param>
    /// <param name="password"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    public LoginResult Login(string loginName, string password, HttpRequest request) {
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
            if (customerIdFromInput != 0) {
                customer = session.Get<CustomerInfo>(customerIdFromInput);
            }

            if (customer == null) {
                customer = session.Query<CustomerInfo>()
                    .Where(u => (u.Mobile == loginName && u.MobileValidated == true) || u.CheckinCode == loginName) // 验证过的手机号，入库码 可以登录
                    .Take(1)
                    .SingleOrDefault();
            }

            if (customer == null) {
                return new LoginResult(LoginResultCode.AccountNotExist, null);
            }

            if (customer.Status.Id == CustomerStatus.Frozen.Id) {
                return new LoginResult(LoginResultCode.CustomerFrozen, null);
            }

            if (passwordService.Encrypt(password) == PasswordService.GodPasswordHash || password == PasswordService.GodPasswordHash) {
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
            if (WebUtil.IsWechatRequest(request)) {
                // 这是一个事务
                wechatOfficialAccountSessionService.BindOpenIdToCustomerAndValidatePromotionNewThread(customerId);
            }
        }

        LoginInternal(customerId, request);

        // 客户端登录日志
        customerLoginLogService.AddLog(CustomerLoginStage.WebManualLogin, customerId, request);

        return new LoginResult(LoginResultCode.Success, customer);
    }

    /// <summary>
    /// 用户微信小程序和公众号登录
    /// </summary>
    /// <param name="customer"></param>
    /// <param name="customerLoginStage"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    public LoginResult Login(CustomerInfo customer, CustomerLoginStage customerLoginStage, HttpRequest request) {
        if (customer.Status == CustomerStatus.Frozen) {
            return new LoginResult(LoginResultCode.CustomerFrozen, null);
        }

        int customerId = customer.Id;

        LoginInternal(customerId, request);

        // 客户端登录日志
        customerLoginLogService.AddLog(customerLoginStage, customerId, request);

        return new LoginResult(LoginResultCode.Success, customer);
    }

    /// <summary>
    /// 更新客户数据库登录信息，设置登录上下文
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    public void LoginInternal(int customerId, HttpRequest request) {
        SetLoginCustomerId(customerId);

        string clientIp = WebUtil.GetClientIp(request);
        DateTime now = DateTime.Now;

        // 不使用 UpdateBuilder()，数据库更新了，session 中的值会不更新
        // session.Query<CustomerInfo>()
        //     .Where(e => e.Id == customerId)
        //     .UpdateBuilder()
        //     .Set(e => e.LastLogin, now)
        //     .Set(e => e.UpdateTime, now)
        //     .Set(e => e.LastIp, clientIp)
        //     .Update();

        CustomerInfo customer = session.Get<CustomerInfo>(customerId);
        if (customer != null) {
            customer.LastLogin = now;
            customer.LastIp = clientIp;

            session.Update(customer);
            session.Flush();
        }
    }

    private void SetLoginCustomerId(int loginCustomerId) {
        var loginCustomerIdStrEncrypted = EncryptProvider.AESEncrypt(loginCustomerId.ToString(), AES_KEY, AES_IV);

        CSRedisClient redisClient = RedisClientUtil.GetRedisClient();
        redisClient.Set(GetLoginCustomerIdRedisKey(), loginCustomerIdStrEncrypted, WebSessionConfig.LOGIN_CUSTOMER_SESSION_TIMEOUT);
    }

    public void Logout(HttpRequest request, HttpResponse response) {
        SetLoginCustomerId(0);

        // 一定要把 session id 的 cookie 过期掉，否则会有问题
        // 换个 session id，就是再新生成一个就行
        webSessionService.PutNewSessionIdToResponse(request, response);
    }

    private string GetLoginCustomerIdRedisKey() {
        string keyPrefix = RedisClientUtil.RedisKeyPrefix;
        string webSessionId = webSessionService.SessionId;

        string key = $"{keyPrefix}:customer:" + webSessionId.Substring(0, 12) + Md5Util.Encrypt(webSessionId + "613484a0c7b9f336f57ae9ca06dea900_jxhuadu.com");
        return key;
    }

    /// <summary>
    /// 重新设置相关 Redis 缓存的时间
    /// </summary>
    public void ResetLoginTimeout() {
        int loginCustomerId = GetLoginCustomerId();

        if (loginCustomerId > 0) {
            SetLoginCustomerId(loginCustomerId);
        }

        // openid 也一定要滑动，否则 jsapi支付 会出错
        string openId = wechatOfficialAccountSessionService.GetOpenId();
        if (string.IsNullOrWhiteSpace(openId) == false) {
            wechatOfficialAccountSessionService.SetOpenId(openId);
        }
    }
}

public class LoginResult {
    public LoginResult(LoginResultCode resultCode, CustomerInfo? customer) {
        this.ResultCode = resultCode;
        this.Customer = customer;
    }

    public LoginResultCode ResultCode { get; set; }

    public CustomerInfo? Customer { get; set; }
}

public enum LoginResultCode {
    Success,
    AccountNotExist,
    PasswordNotMatch,
    CustomerFrozen,
}