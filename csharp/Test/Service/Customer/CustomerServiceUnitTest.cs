using NHibernate;
using shunda.Core.Config;
using shunda.Core.Hibernate;
using shunda.Domain;
using shunda.Service.Customer;
using shunda.Service.Pass;

namespace shunda.Test.Service.Customer;

public class CustomerServiceUnitTest
{
    [Test]
    public void TestCreateCustomerByOpenId()
    {
        ISessionFactory sessionFactory = HibernateSessionFactory.BuildSessionFactory(DataBaseConfig.DataBaseType.MySQL, new DomainMock().GetType().Assembly);
        ISession session = HibernateSessionFactory.OpenSession(sessionFactory);

        CustomerCheckinCodeService customerCheckinCodeService = new CustomerCheckinCodeService(session);
        PasswordService passwordService = new PasswordService();

        CustomerService customerService = new CustomerService(session, customerCheckinCodeService, passwordService);

        customerService.CreateCustomerByOpenId("open-id-test-0001");
    }
}