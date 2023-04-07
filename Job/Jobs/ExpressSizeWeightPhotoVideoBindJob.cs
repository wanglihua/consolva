using NHibernate;
using Quartz;
using shunda.Core.Hibernate;
using shunda.Core.Service;
using shunda.Domain.Express;
using ISession = NHibernate.ISession;

namespace shunda.Job.Jobs;

[Service(ServiceType.Singleton)]
public class ExpressSizeWeightPhotoVideoBindJob : IJob {
    private readonly ISessionFactory sessionFactory;

    public ExpressSizeWeightPhotoVideoBindJob(ISessionFactory sessionFactory) {
        this.sessionFactory = sessionFactory;
    }

    public static void Schedule(IScheduler scheduler, DateTime jobStartTime) {
        IJobDetail jobDetail = JobBuilder.Create<ExpressSizeWeightPhotoVideoBindJob>().WithDescription("ExpressSizeWeightPhotoVideoBindJob").Build();
        ITrigger jobTrigger = TriggerBuilder.Create()
            .StartAt(new DateTimeOffset(jobStartTime, new TimeSpan(8, 0, 0))) // 暂时按中国区时间
            .WithSimpleSchedule(x => x.WithIntervalInHours(12).RepeatForever())
            .Build();

        scheduler.ScheduleJob(jobDetail, jobTrigger).Wait();
    }

    public Task Execute(IJobExecutionContext context) {
        ISession session = HibernateSessionFactory.OpenSession(sessionFactory);

        DateTime yesterdayBegin = DateTime.Today.AddDays(-1);

        Transactional.Wrap(session, () => {
            // 先按尺寸重量，图片，视频
            var sizeWeightList = session.Query<ExpressSizeWeightInfo>()
                .Where(e => e.UpdateTime >= yesterdayBegin)
                .ToList();

            foreach (var sizeWeight in sizeWeightList) {
                string trackingNo = sizeWeight.TrackingNo;
                var express = session.Query<ExpressInfo>()
                    .Where(e => e.TrackingNo == trackingNo)
                    .Take(1)
                    .SingleOrDefault();

                sizeWeight.Express = express; // 可能为 null
                sizeWeight.Order = express?.Order; // 可能为 null

                session.Update(sizeWeight);
            }

            session.Flush();

            var photoList = session.Query<ExpressPhotoInfo>()
                .Where(e => e.UpdateTime >= yesterdayBegin)
                .ToList();

            foreach (var photo in photoList) {
                if (photo.Order == null) {
                    string trackingNo = photo.TrackingNo;
                    var express = session.Query<ExpressInfo>()
                        .Where(e => e.TrackingNo == trackingNo)
                        .Take(1)
                        .SingleOrDefault();

                    photo.Express = express; // 可能为 null
                    photo.Order = express?.Order; // 可能为 null
                    photo.Customer = express?.Customer;

                    session.Update(photo);
                }
            }

            session.Flush();

            var vidoeList = session.Query<ExpressVideoInfo>()
                .Where(e => e.UpdateTime >= yesterdayBegin)
                .ToList();

            foreach (var video in vidoeList) {
                if (video.Order != null) {
                    string trackingNo = video.TrackingNo;
                    var express = session.Query<ExpressInfo>()
                        .Where(e => e.TrackingNo == trackingNo)
                        .Take(1)
                        .SingleOrDefault();

                    video.Express = express; // 可能为 null
                    video.Order = express?.Order; // 可能为 null
                    video.Customer = express?.Customer;

                    session.Update(video);
                }
            }

            session.Flush();

            // 再按快递单号
            var recentExpressList = session.Query<ExpressInfo>()
                .Where(e => e.UpdateTime >= yesterdayBegin)
                .ToList();

            foreach (var express in recentExpressList) {
                string trackingNo = express.TrackingNo;

                var expressSizeWeightList = session.Query<ExpressSizeWeightInfo>()
                    .Where(e => e.TrackingNo == trackingNo)
                    .ToList();
                foreach (var sizeWeight in expressSizeWeightList) {
                    sizeWeight.Express = express;
                    sizeWeight.Order = express.Order; // 可能为 null

                    session.Update(sizeWeight);
                }

                var expressPhotoList = session.Query<ExpressPhotoInfo>()
                    .Where(e => e.TrackingNo == trackingNo)
                    .ToList();
                foreach (var photo in expressPhotoList) {
                    photo.Express = express;
                    photo.Order = express.Order; // 可能为 null
                    photo.OrderCode = express.Order?.Code ?? string.Empty;
                    photo.Customer = express.Customer;

                    session.Update(photo);
                }

                var expressVideoList = session.Query<ExpressVideoInfo>()
                    .Where(e => e.TrackingNo == trackingNo)
                    .ToList();
                foreach (var video in expressVideoList) {
                    video.Express = express;
                    video.Order = express.Order; // 可能为 null
                    video.OrderCode = express.Order?.Code ?? string.Empty;
                    video.Customer = express.Customer;

                    session.Update(video);
                }

                session.Flush();
            }
        });

        return Task.CompletedTask;
    }
}