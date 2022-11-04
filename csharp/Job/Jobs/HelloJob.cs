using NHibernate;
using NLog;
using Quartz;
using shunda.Core.Hibernate;
using shunda.Core.Service;
using shunda.Domain.Pass;
using ISession = NHibernate.ISession;

namespace shunda.Job.Jobs;

[Service(ServiceType.Singleton)]
public class HelloJob : IJob
{
    private static Logger log = LogManager.GetCurrentClassLogger();

    private ISessionFactory sessionFactory;

    public HelloJob(ISessionFactory sessionFactory)
    {
        this.sessionFactory = sessionFactory;
    }

    public static void Schedule(IScheduler scheduler)
    {
        var jobDetail = JobBuilder.Create<HelloJob>().WithDescription("HelloJob").Build();
        // var sampleTrigger = TriggerBuilder.Create().StartNow().WithCronSchedule("* * * * * ? *").Build();
        var jobTrigger = TriggerBuilder.Create()
            .StartAt(new DateTimeOffset(DateTime.Parse("2022-01-01 04:00"), new TimeSpan(8, 0, 0)))
            .WithSimpleSchedule(
                x => x.WithIntervalInSeconds(1).RepeatForever()
            )
            .Build();
        // await scheduler.ScheduleJob(sampleJob, sampleTrigger);
        scheduler.ScheduleJob(jobDetail, jobTrigger).Wait();
    }

    public Task Execute(IJobExecutionContext context)
    {
        ISession session = null;
        ITransaction transaction = null;
        try
        {
            session = sessionFactory.WithOptions().Interceptor(new HibernateInterceptor()).OpenSession();

            transaction = session.BeginTransaction();

            IList<GroupInfo> groupList = session.Query<GroupInfo>().ToList();

            log.Trace("group count " + groupList.Count);

            log.Trace("Hello World from HelloJob！" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

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

        return Task.CompletedTask;
    }
}