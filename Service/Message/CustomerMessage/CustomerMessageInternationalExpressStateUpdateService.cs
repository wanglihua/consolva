using NHibernate;
using shunda.Core.Service;
using shunda.Domain.Message;
using shunda.Service.Customer;

namespace shunda.Service.Message.CustomerMessage;

/// <summary>
/// 国际快递更新提醒
/// </summary>
[Service(ServiceType.Scoped)]
public class CustomerMessageInternationalExpressStateUpdateService {
    private readonly ISession session;
    private readonly CustomerMessageCommonService customerMessageCommonService;
    private readonly CustomerCodeService customerCodeService;

    public CustomerMessageInternationalExpressStateUpdateService(
        ISession session,
        CustomerMessageCommonService customerMessageCommonService,
        CustomerCodeService customerCodeService
    ) {
        this.session = session;
        this.customerMessageCommonService = customerMessageCommonService;
        this.customerCodeService = customerCodeService;
    }
    
    private static CustomerMessageType messageType => CustomerMessageType.InternationalExpressStateUpdate;    
}