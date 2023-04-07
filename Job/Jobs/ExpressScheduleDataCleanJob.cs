using NHibernate;
using NLog;
using Quartz;
using shunda.Core.Hibernate;
using shunda.Core.Service;
using ISession = NHibernate.ISession;

namespace shunda.Job.Jobs;

[Service(ServiceType.Singleton)]
public class ExpressScheduleDataCleanJob : IJob {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISessionFactory sessionFactory;

    public ExpressScheduleDataCleanJob(ISessionFactory sessionFactory) {
        this.sessionFactory = sessionFactory;
    }

    public static void Schedule(IScheduler scheduler, DateTime jobStartTime) {
        var jobDetail = JobBuilder.Create<ExpressScheduleDataCleanJob>().WithDescription("ExpressScheduleDataCleanJob").Build();
        var jobTrigger = TriggerBuilder.Create()
            .StartAt(new DateTimeOffset(jobStartTime, new TimeSpan(8, 0, 0)))
            .WithSimpleSchedule(
                x => x.WithIntervalInHours(24).RepeatForever()
            )
            .Build();
        scheduler.ScheduleJob(jobDetail, jobTrigger).Wait();
    }

    public Task Execute(IJobExecutionContext context) {
        ISession session = HibernateSessionFactory.OpenSession(sessionFactory);

        Transactional.Wrap(session, () => {
            string sql1 = @"
DELETE
FROM tbl_express_shelf_schedule ess
WHERE ess.create_time < DATE_SUB(DATE(NOW()), INTERVAL 30 DAY)
";

            string sql2 = @"
DELETE
FROM tbl_express_dump_schedule eds
WHERE eds.create_time < DATE_SUB(DATE(NOW()), INTERVAL 30 DAY)
";
            session.CreateSQLQuery(sql1).List();
            session.CreateSQLQuery(sql2).List();
        });

        return Task.CompletedTask;
    }
}