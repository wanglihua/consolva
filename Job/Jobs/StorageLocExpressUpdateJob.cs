using NHibernate;
using NLog;
using Quartz;
using shunda.Core.Hibernate;
using shunda.Core.Service;
using shunda.Service.Express;
using ISession = NHibernate.ISession;

namespace shunda.Job.Jobs;

/// <summary>
/// 更新仓位快递单记录
/// 作为一种补偿纠正
/// </summary>
[Service(ServiceType.Singleton)]
public class StorageLocExpressUpdateJob : IJob {
    private static Logger log = LogManager.GetCurrentClassLogger();

    private ISessionFactory sessionFactory;

    public StorageLocExpressUpdateJob(ISessionFactory sessionFactory) {
        this.sessionFactory = sessionFactory;
    }

    public static void Schedule(IScheduler scheduler, DateTime jobStartTime) {
        IJobDetail jobDetail = JobBuilder.Create<StorageLocExpressUpdateJob>().WithDescription("StorageLocExpressUpdateJob").Build();
        ITrigger jobTrigger = TriggerBuilder.Create()
            .StartAt(new DateTimeOffset(jobStartTime, new TimeSpan(8, 0, 0))) // 暂时按中国区时间
            .WithSimpleSchedule(x => x.WithIntervalInHours(24).RepeatForever())
            .Build();

        scheduler.ScheduleJob(jobDetail, jobTrigger).Wait();
    }

    public Task Execute(IJobExecutionContext context) {
        DateTime beginTime = DateTime.Now;
        log.Warn("StorageLocExpressUpdateJob Begin");

        ISession? session = null;
        try {
            session = HibernateSessionFactory.OpenSession(sessionFactory);

            StorageLocExpressService storageLocExpressService = new StorageLocExpressService(session);

            storageLocExpressService.UpdateAll(); // 内部有事务处理

            session.Flush();
        }
        catch (Exception exp) {
            log.Error(exp);
        }
        finally {
            session?.Close();
        }

        DateTime endTime = DateTime.Now;
        log.Warn("StorageLocExpressUpdateJob End");
        log.Warn("StorageLocExpressUpdateJob Total Seconds: " + (endTime - beginTime).TotalSeconds);

        return Task.CompletedTask;
    }
}