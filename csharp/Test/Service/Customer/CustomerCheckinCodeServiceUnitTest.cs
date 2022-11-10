using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using NHibernate;
using shunda.Core.Config;
using shunda.Core.Hibernate;
using shunda.Domain;
using shunda.Service.Customer;

namespace shunda.Test.Service.Customer;

public class CustomerCheckinCodeServiceUnitTest
{
    [Test]
    public void TestRandCode()
    {
        // ISessionFactory sessionFactory = HibernateSessionFactory.BuildSessionFactory(DataBaseConfig.DataBaseType.MySQL, new DomainMock().GetType().Assembly);
        // ISession session = HibernateSessionFactory.OpenSession(sessionFactory);

        var summary = BenchmarkRunner.Run<CustomerCheckinCodeServiceBenchmark>(DefaultConfig.Instance.WithOptions(ConfigOptions.DisableOptimizationsValidator));
    }

    // [HtmlExporter]
    public class CustomerCheckinCodeServiceBenchmark
    {
        private CustomerCheckinCodeService customerCheckinCodeService = new CustomerCheckinCodeService(null);

        [Benchmark()]
        public void RandCode()
        {
            string code = customerCheckinCodeService.RandCode();
        }
    }
}