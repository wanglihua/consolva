using shunda.Core.Service;
using shunda.Domain;
using shunda.Domain.Customer;
using shunda.Domain.Wechat;
using shunda.Service.Pass;
using ISession = NHibernate.ISession;

namespace shunda.Service.Customer;

[Service(ServiceType.Scoped)]
public class CustomerService {
    private readonly ISession session;
    private readonly CustomerCheckinCodeService customerCheckinCodeService;
    private readonly PasswordService passwordService;

    public CustomerService(ISession session,
        CustomerCheckinCodeService customerCheckinCodeService,
        PasswordService passwordService) {
        this.session = session;
        this.customerCheckinCodeService = customerCheckinCodeService;
        this.passwordService = passwordService;
    }

    /// <summary>
    /// 通过小程序创建客户
    /// </summary>
    public CustomerInfo CreateCustomerByWechatApp(string wechatAppOpenId, string wechatUnionId) {
        if (string.IsNullOrWhiteSpace(wechatAppOpenId) == true) {
            throw new Exception("通过微信小程序创建客户，微信小程序OpenId不能都为空！");
        }

        wechatAppOpenId = wechatAppOpenId.Trim();
        wechatUnionId = wechatUnionId.Trim();

        var customerExistQuery = session.Query<CustomerInfo>();

        if (string.IsNullOrWhiteSpace(wechatAppOpenId) == false) {
            customerExistQuery = customerExistQuery.Where(e => e.WechatAppOpenId == wechatAppOpenId);
        }

        var customerExist = customerExistQuery
            .Take(1)
            .SingleOrDefault();

        if (customerExist != null) {
            if (string.IsNullOrWhiteSpace(wechatAppOpenId) == false) {
                throw new Exception($"微信小程序OpenId {wechatAppOpenId} 对应的客户已经存在！");
            }
        }

        var follower = session.Query<WechatOfficialAccountFollowerInfo>()
            .Where(e => e.UnionId == wechatUnionId)
            .Take(1)
            .SingleOrDefault();

        return CreateCustomerByOpenId(wechatAppOpenId, wechatUnionId, follower);
    }

    /// <summary>
    /// 通过微信公众号创建客户
    /// </summary>
    public CustomerInfo CreateCustomerByWechatOfficialAccount(WechatOfficialAccountFollowerInfo follower) {
        return CreateCustomerByOpenId(string.Empty, follower.UnionId, follower);
    }

    /// <summary>
    /// 通过 微信小程序open id 和 公众号关注信息 自动创建客户
    /// 这里只管客户的创建，创建后的自动登录等的，有别的逻辑完成
    /// </summary>
    /// <returns>新创建的客户</returns>
    private CustomerInfo CreateCustomerByOpenId(string wechatAppOpenId, string wechatUnionId, WechatOfficialAccountFollowerInfo? follower) {
        string checkinCode = customerCheckinCodeService.GenerateNewCheckinCode();
        string salt = passwordService.GenerateSalt();
        DateTime now = DateTime.Now;
        CustomerInfo customer = new CustomerInfo() {
            CheckinCode = checkinCode,
            Country = Country.None,
            WechatAppOpenId = wechatAppOpenId,
            WechatUnionId = wechatUnionId,
            Mobile = string.Empty,
            MobileValidated = false,
            Salt = salt,
            Password = string.Empty,
            PayPassword = string.Empty,
            RegTime = now,
            LastLogin = now,
            LastIp = string.Empty,
            Account = 0m, // 这里是创建时没有赠送余额
            TotalScore = 0m,
            ValidScore = 0m,
            TotalConsume = 0m,
            Level = CustomerLevel.Common,
            VipRank = CustomerVipRank.None,
            CustomerCharacter = string.Empty,
            CreateMethod = CustomerCreateMethod.CreateByOpenId,
            Status = CustomerStatus.Normal,
        };

        if (follower != null) {
            customer.WechatOfficialAccountOpenId = follower.OpenId;
            customer.WechatUnionId = follower.UnionId; // 保证 公众号创建的时候也不会忘记设置这个，双保险
            customer.WechatOfficialAccountFollowStatus = follower.FollowStatus;
            customer.WechatOfficialAccountFollowTime = follower.FollowTime;
            customer.WechatOfficialAccountUnfollowTime = follower.UnfollowTime;
        }

        session.Save(customer);
        session.Flush();

        int customerId = customer.Id;

        // 创建客户后，必须要调用这个，否则会有入库码重复的可能
        customerCheckinCodeService.SaveCheckinCodeToCache(checkinCode, customerId);

        session.Flush();

        return customer;
    }
}