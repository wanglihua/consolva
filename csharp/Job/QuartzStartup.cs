using System;
using System.Collections.Specialized;
using NLog;
using Quartz;
using Quartz.Impl;
using shunda.Core.Quartz;
using shunda.Job.Jobs;

namespace shunda.Job;

public class QuartzStartup
{
    private static Logger log = LogManager.GetCurrentClassLogger();

    private IServiceProvider serviceProvider;
    private IScheduler? scheduler;

    private bool isDevelopment = false;

    public QuartzStartup(IServiceProvider serviceProvider, bool isDevelopment)
    {
        this.serviceProvider = serviceProvider;
        this.isDevelopment = isDevelopment;
    }

    // public async Task Start()
    public void Start()
    {
        if (scheduler != null)
        {
            throw new InvalidOperationException("Already started.");
        }

        var properties = new NameValueCollection {
            ["quartz.serializer.type"] = "binary",
        };

        var schedulerFactory = new StdSchedulerFactory(properties);

        // scheduler = await schedulerFactory.GetScheduler();
        scheduler = schedulerFactory.GetScheduler().Result;
        scheduler.JobFactory = new JobFactory(serviceProvider);
        // await scheduler.Start();
        scheduler.Start().Wait();

        if (isDevelopment)
        {
            HelloJob.Schedule(scheduler);
        }
        else
        {
            // 下面的定时任务需要错开时间开始

            DateTime jobStartTime = DateTime.Parse("2022-01-01 02:00"); // 这个时间是 UTC 时间，加上 new TimeSpan(10, 0, 0) 变成 澳大利亚时间
            // int jobIntervalMinutes = 5; // 每个任务的错开时间

            // jobStartTime = jobStartTime.AddMinutes(jobIntervalMinutes);
            // ExchangeRateRefreshJob.Schedule(scheduler, jobStartTime);
        }
    }

    public void Stop()
    {
        if (scheduler == null)
        {
            return;
        }

        // give running jobs 10 sec to stop gracefully
        if (scheduler.Shutdown(waitForJobsToComplete: true).Wait(10000))
        {
            scheduler = null;
        }
        else
        {
            log.Error("系统关闭，但有运行中的定时任务未能完成！");
        }
    }
}