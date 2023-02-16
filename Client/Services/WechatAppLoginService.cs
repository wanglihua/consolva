using NHibernate.Criterion;
using shunda.Core.Service;
using shunda.Domain.Customer;
using shunda.Domain.Wechat;
using shunda.Service.Customer;
using shunda.Service.Wechat;
using ISession = NHibernate.ISession;

namespace shunda.Client.Services;

[Service(ServiceType.Scoped)]
public class WechatAppLoginService {
    private readonly ISession session;
    private readonly WechatAppAuthCodeSessionService wechatAppAuthCodeSessionService;
    private readonly CustomerService customerService;
    private readonly LoginCustomerService loginCustomerService;
    private readonly WechatOfficialAccountFollowerService wechatOfficialAccountFollowerService;

    public WechatAppLoginService(
        ISession session,
        WechatAppAuthCodeSessionService wechatAppAuthCodeSessionService,
        CustomerService customerService,
        LoginCustomerService loginCustomerService,
        WechatOfficialAccountFollowerService wechatOfficialAccountFollowerService
    ) {
        this.session = session;
        this.wechatAppAuthCodeSessionService = wechatAppAuthCodeSessionService;
        this.customerService = customerService;
        this.loginCustomerService = loginCustomerService;
        this.wechatOfficialAccountFollowerService = wechatOfficialAccountFollowerService;
    }

    /// <summary>
    /// 微信小程序登录
    /// </summary>
    /// <param name="code">wx.login() 获得的 code</param>
    /// <param name="request"></param>
    public void Login(string code, HttpRequest request) {
        var result = wechatAppAuthCodeSessionService.GetResult(code);
        string wechatAppOpenId = result.OpenId;
        string wechatUnionId = result.UnionId;

        ICriterion criterion = Restrictions.Where<CustomerInfo>(e => e.WechatAppOpenId == wechatAppOpenId);
        if (string.IsNullOrWhiteSpace(wechatUnionId) == false) // wechatUnionId 非空时才能带上这个查询 
        {
            criterion = Restrictions.Or(criterion, Restrictions.Where<CustomerInfo>(e => e.WechatUnionId == wechatUnionId));
        }

        var customer = session.QueryOver<CustomerInfo>()
            .Where(criterion)
            .Take(1)
            .SingleOrDefault();

        if (customer != null) {
            bool shouldUpdateCustomer = false;

            // 这里可能是更新由关注公众号创建的客户的信息，也可能原来没有union id的更新 union_id，这种可能性很小s
            if (customer.WechatAppOpenId != wechatAppOpenId || customer.WechatUnionId != wechatUnionId) {
                customer.WechatAppOpenId = wechatAppOpenId;
                customer.WechatUnionId = wechatUnionId;

                shouldUpdateCustomer = true;
            }

            // 可能的更新微信公众号 open id
            if (string.IsNullOrWhiteSpace(customer.WechatOfficialAccountOpenId)) {
                var follower = session.Query<WechatOfficialAccountFollowerInfo>()
                    .Where(e => e.UnionId == wechatUnionId)
                    .Take(1)
                    .SingleOrDefault();

                if (follower != null) {
                    customer.WechatOfficialAccountOpenId = follower.OpenId;
                    customer.WechatOfficialAccountFollowStatus = follower.FollowStatus;
                    customer.WechatOfficialAccountFollowTime = follower.FollowTime;
                    customer.WechatOfficialAccountUnfollowTime = follower.UnfollowTime;

                    shouldUpdateCustomer = true;
                }
            }

            if (shouldUpdateCustomer == true) {
                session.Update(customer);
                session.Flush();
            }
        }

        if (customer == null) {
            customer = customerService.CreateCustomerByWechatApp(wechatAppOpenId, wechatUnionId);
        }

        loginCustomerService.Login(customer, CustomerLoginStage.WechatAppLogin, request);
    }
}