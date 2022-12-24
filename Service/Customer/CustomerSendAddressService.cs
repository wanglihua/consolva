using shunda.Core.Service;
using ISession = NHibernate.ISession;

namespace shunda.Service.Customer;

[Service(ServiceType.Scoped)]
public class CustomerSendAddressService {
    private readonly ISession session;

    public CustomerSendAddressService(ISession session) {
        this.session = session;
    }
}