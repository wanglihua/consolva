// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Running;
using shunda.Benchmark.Core;
using shunda.Benchmark.Service.Customer;

Console.WriteLine("Hello, World!");

// ISessionFactory sessionFactory = HibernateSessionFactory.BuildSessionFactory(DataBaseConfig.DataBaseType.MySQL, new DomainMock().GetType().Assembly);
// ISession session = HibernateSessionFactory.OpenSession(sessionFactory);

// var summary = BenchmarkRunner.Run<CustomerCheckinCodeServiceBenchmark>();
var summary = BenchmarkRunner.Run<CaptchaServiceBenchmark>();