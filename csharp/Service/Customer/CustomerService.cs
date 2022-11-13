using shunda.Core.Service;
using shunda.Domain;
using shunda.Domain.Customer;
using shunda.Service.Pass;
using ISession = NHibernate.ISession;

namespace shunda.Service.Customer;

[Service(ServiceType.Scoped)]
public class CustomerService
{
    private readonly ISession session;
    private readonly CustomerCheckinCodeService customerCheckinCodeService;
    private readonly PasswordService passwordService;

    public CustomerService(ISession session,
        CustomerCheckinCodeService customerCheckinCodeService,
        PasswordService passwordService)
    {
        this.session = session;
        this.customerCheckinCodeService = customerCheckinCodeService;
        this.passwordService = passwordService;
    }

    /// <summary>
    /// 通过 open id 自动创建客户
    /// 这里只管客户的创建，创建后的自动登录等的，有别的逻辑完成
    /// </summary>
    /// <param name="openId"></param>
    /// <returns>新创建的客户id</returns>
    public int CreateCustomerByOpenId(string openId)
    {
        if (string.IsNullOrWhiteSpace(openId))
        {
            throw new Exception("通过open id 创建客户，open id 不能为空！");
        }

        openId = openId.Trim();
        int customerIdExist = session.Query<CustomerInfo>()
            .Where(e => e.OpenId == openId)
            .Select(e => e.Id)
            .Take(1)
            .SingleOrDefault();

        if (customerIdExist != 0)
        {
            throw new Exception($"open id #{openId} 对应的客户已经存在！");
        }

        string checkinCode = customerCheckinCodeService.GenerateNewCheckinCode();
        string salt = passwordService.GenerateSalt();
        DateTime now = DateTime.Now;
        CustomerInfo customer = new CustomerInfo() {
            CheckinCode = checkinCode,
            Country = Country.None,
            OpenId = openId,
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

        session.Save(customer);
        session.Flush();

        int customerId = customer.Id;
        customerCheckinCodeService.SaveCheckinCodeToCache(checkinCode, customerId);
        
        session.Flush();

        return customerId;
    }
}