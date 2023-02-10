using NHibernate;
using shunda.Core.Quartz;
using shunda.Core.Service;

namespace shunda.Job.Jobs;

[Service(ServiceType.Singleton)]
public class ExpressSizeWeightPhotoVideoBindJob : JobBase {
    public ExpressSizeWeightPhotoVideoBindJob(ISessionFactory sessionFactory) : base(sessionFactory) {
    }

    public override void Execute() {
        throw new NotImplementedException();
    }
}