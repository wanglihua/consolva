using NHibernate;
using NHibernate.AdoNet;
using NHibernate.Engine;

namespace shunda.Core.Hibernate.MiniProfilerMySql;

public class MiniProfilerMySqlClientBatchingBatcherFactory : IBatcherFactory
{
    public virtual IBatcher CreateBatcher(ConnectionManager connectionManager, IInterceptor interceptor)
    {
        return new MiniProfilerMySqlClientBatchingBatcher(connectionManager, interceptor);
    }
}