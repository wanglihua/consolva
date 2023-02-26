using NLog;
using NLog.Fluent;
using Quartz;
using Quartz.Spi;

namespace shunda.Core.Quartz;

#nullable disable

public class JobFactory : IJobFactory
{
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private IServiceProvider serviceProvider = null;

    public JobFactory(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
    {
        if (bundle == null || bundle.JobDetail == null)
        {
            log.Error("bundle == null || bundle.JobDetail == null");

            return null;
        }

        string jobDescription = bundle.JobDetail.Description ?? string.Empty;

        string jobKey = bundle.JobDetail.Key.Name;
        if (string.IsNullOrWhiteSpace(jobKey))
        {
            jobKey = string.Empty;
        }

        IJob job = serviceProvider.GetService(bundle.JobDetail.JobType) as IJob;
        if (job == null)
        {
            log.Error($"job ({jobDescription}) 创建失败！Job Key: {jobKey}");
            return null;
        }

        return job;
    }

    public void ReturnJob(IJob job)
    {
        (job as IDisposable)?.Dispose();
    }
}