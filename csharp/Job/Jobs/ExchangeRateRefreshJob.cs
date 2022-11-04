using NHibernate;
using NLog;
using Quartz;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Service;
using shunda.Service;
using ISession = NHibernate.ISession;

namespace shunda.Job.Jobs;

/// <summary>
/// 更新汇率
/// </summary>
[Service(ServiceType.Singleton)]
public class ExchangeRateRefreshJob : IJob
{
    private static Logger log = LogManager.GetCurrentClassLogger();

    private ISessionFactory sessionFactory = null;

    public ExchangeRateRefreshJob(ISessionFactory sessionFactory)
    {
        this.sessionFactory = sessionFactory;
    }

    public static void Schedule(IScheduler scheduler, DateTime jobStartTime)
    {
        IJobDetail jobDetail = JobBuilder.Create<ExchangeRateRefreshJob>().WithDescription("ExchangeRateRefreshJob").Build();
        ITrigger jobTrigger = TriggerBuilder.Create()
            .StartAt(new DateTimeOffset(jobStartTime, new TimeSpan(10, 0, 0))) // 澳大利亚时区是在 东10区
            // .WithSimpleSchedule(x => x.WithIntervalInHours(12).RepeatForever())
            .WithSimpleSchedule(x => x.WithIntervalInMinutes(30).RepeatForever())
            .Build();

        scheduler.ScheduleJob(jobDetail, jobTrigger).Wait();
    }

    public Task Execute(IJobExecutionContext context)
    {
        DateTime beginTime = DateTime.Now;
        log.Warn("ExchangeRateRefreshJob Begin");

        ISession? session = null;
        try
        {
            session = HibernateSessionFactory.OpenSession(sessionFactory);

            ExchangeRateService exchangeRateService = new ExchangeRateService(session,
                new HttpRequestService(),
                new SiteErrorService(session)
            );

            exchangeRateService.RefreshRate(useInnerTransaction: true); // 内部有事务处理

            session.Flush();
        }
        catch (Exception exp)
        {
            log.Error(exp);
        }
        finally
        {
            session?.Close();
        }

        DateTime endTime = DateTime.Now;
        log.Warn("ExchangeRateRefreshJob End");
        log.Warn("ExchangeRateRefreshJob Total Seconds: " + (endTime - beginTime).TotalSeconds);

        return Task.CompletedTask;
    }
}