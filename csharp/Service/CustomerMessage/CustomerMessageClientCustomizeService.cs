
using NHibernate;
using shunda.Core.Service;
using shunda.Domain.Message;
using ISession = NHibernate.ISession;

namespace shunda.Service.CustomerMessage;

[Service(ServiceType.Scoped)]
public class CustomerMessageClientCustomizeService
{
    private ISession session;

    public CustomerMessageClientCustomizeService(ISession session)
    {
        this.session = session;
    }

    public bool ShouldSend(int customerId, CustomerMessageType messageType)
    {
        
        
        var clientCustomize = session.Query<CustomerMessageClientCustomizeInfo>()
            .Where(e => e.CustomerId == customerId && e.MessageType == messageType)
            .Take(1)
            .SingleOrDefault();

        if (clientCustomize == null)
        {
            return true;
        }

        return clientCustomize.Send;
    }
}