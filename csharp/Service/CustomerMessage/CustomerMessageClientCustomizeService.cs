using System.Linq;
using Core;
using Domain.Message;
using NHibernate;

namespace shunda.Service.CustomerMessage
{
    [Service(ServiceType.Scoped)]
    public class CustomerMessageClientCustomizeService
    {
        private ISession session = null;

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
}