using NHibernate;
using NLog;
using Quartz;
using shunda.Core.Quartz;
using shunda.Core.Service;
using shunda.Domain.Pass;

namespace shunda.Job.Jobs;

[Service(ServiceType.Singleton)]
public class HelloJob : JobBase
{
    private static Logger log = LogManager.GetCurrentClassLogger();

    public HelloJob(ISessionFactory sessionFactory) : base(sessionFactory)
    {
    }

    public static void Schedule(IScheduler scheduler)
    {
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

    public override void Execute()
    {
        IList<GroupInfo> groupList = session.Query<GroupInfo>().ToList();

        log.Trace("group count " + groupList.Count);

        log.Trace("Hello World from HelloJob！" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
    }
}