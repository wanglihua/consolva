using NHibernate;

namespace shunda.Core.Hibernate;

/// <summary>
/// 使用
/// NHibernateLogger.SetLoggersFactory(new HibernateLoggerFactory());
/// </summary>
public class HibernateLoggerFactory: INHibernateLoggerFactory
{
    public INHibernateLogger LoggerFor(string keyName)
    {
        return new HibernateLogger();
    }

    public INHibernateLogger LoggerFor(Type type)
    {
        return new HibernateLogger();
    }
}