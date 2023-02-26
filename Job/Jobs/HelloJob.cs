using NHibernate;
using NLog;
using Quartz;
using shunda.Core.Hibernate;
using shunda.Core.Service;
using shunda.Domain.Pass;
using ISession = NHibernate.ISession;

namespace shunda.Job.Jobs;

[Service(ServiceType.Singleton)]
public class HelloJob : IJob {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISessionFactory sessionFactory;
    private readonly IHttpClientFactory httpClientFactory;

    public HelloJob(ISessionFactory sessionFactory, IHttpClientFactory httpClientFactory) {
        this.sessionFactory = sessionFactory;
        this.httpClientFactory = httpClientFactory;
    }

    public static void Schedule(IScheduler scheduler) {
        var jobDetail = JobBuilder.Create<HelloJob>().WithDescription("HelloJob").Build();
        // var sampleTrigger = TriggerBuilder.Create().StartNow().WithCronSchedule("* * * * * ? *").Build();
        var jobTrigger = TriggerBuilder.Create()
            .StartAt(new DateTimeOffset(DateTime.Parse("2022-01-01 00:00"), new TimeSpan(8, 0, 0)))
            .WithSimpleSchedule(
                x => x.WithIntervalInSeconds(1).RepeatForever()
            )
            .Build();
        // await scheduler.ScheduleJob(sampleJob, sampleTrigger);
        scheduler.ScheduleJob(jobDetail, jobTrigger).Wait();
    }

    public Task Execute(IJobExecutionContext context) {
        HttpClient httpClient = httpClientFactory.CreateClient();

        ISession session = HibernateSessionFactory.OpenSession(sessionFactory);

        Transactional.Wrap(session, () => {
            IList<GroupInfo> groupList = session.Query<GroupInfo>().ToList();

            log.Trace("group count " + groupList.Count);

            log.Trace("Hello World from HelloJob！" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        });

        return Task.CompletedTask;
    }
}