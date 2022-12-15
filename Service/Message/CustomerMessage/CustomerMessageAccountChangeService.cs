using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.Customer;
using shunda.Domain.Express;
using shunda.Domain.Message;
using shunda.Service.Customer;
using ISession = NHibernate.ISession;

namespace shunda.Service.Message.CustomerMessage;

/// <summary>
/// 账户余额变动提醒
/// </summary>
[Service(ServiceType.Scoped)]
public class CustomerMessageAccountChangeService
{
    private readonly ISession session;
    private readonly CustomerMessageCommonService customerMessageCommonService;
    private readonly CustomerClientUrlService customerClientUrlService;
    private readonly CustomerCodeService customerCodeService;
    
    public CustomerMessageAccountChangeService(ISession session,
        CustomerMessageCommonService customerMessageCommonService,
        CustomerClientUrlService customerClientUrlService,
        CustomerCodeService customerCodeService)
    {
        this.session = session;
        this.customerMessageCommonService = customerMessageCommonService;
        this.customerClientUrlService = customerClientUrlService;
        this.customerCodeService = customerCodeService;
    }

    private static CustomerMessageType messageType => CustomerMessageType.AccountChange;

    private string? GetMessageUrl()
    {
        return customerClientUrlService.GetCustomerAccountUrl();
    }    
}