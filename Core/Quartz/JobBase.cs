using NHibernate;
using NLog;
using Quartz;
using shunda.Core.Hibernate;

namespace shunda.Core.Quartz;

public abstract class JobBase : IJob
{
    private static Logger log = LogManager.GetCurrentClassLogger();

    private ISessionFactory sessionFactory;

    protected JobBase(ISessionFactory sessionFactory)
    {
        this.sessionFactory = sessionFactory;
    }

    protected ISession session;

    public Task Execute(IJobExecutionContext context)
    {
        ITransaction transaction = null;
        try
        {
            session = HibernateSessionFactory.OpenSession(sessionFactory);
            transaction = session.BeginTransaction();

            Execute();

            session.Flush();
            transaction.Commit();
        }
        catch (Exception exp)
        {
            transaction?.Rollback();
            log.Error(exp);
        }
        finally
        {
            session?.Close();
        }
        
        
        Transactional.Wrap(session, () => { 
        
        });

        return Task.CompletedTask;
    }

    public abstract void Execute();
}