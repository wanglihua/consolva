using NHibernate;
using NLog;

namespace shunda.Core.Hibernate;

public class HibernateLogger : INHibernateLogger
{
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    public HibernateLogger()
    {
    }

    public void Log(NHibernateLogLevel logLevel, NHibernateLogValues state, Exception exception)
    {
        // 未完整实现，都禁用掉，暂时不需要
        
        // log.Error(state.ToString);
        // log.Error(exception);
    }

    public bool IsEnabled(NHibernateLogLevel logLevel)
    {
        // 未完整实现，都禁用掉，暂时不需要
        return false;
    }
}