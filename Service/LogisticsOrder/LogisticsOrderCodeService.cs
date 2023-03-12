using NHibernate;
using NLog;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Service;
using shunda.Domain.Customer;

namespace shunda.Service.LogisticsOrder;

[Service(ServiceType.Scoped)]
public class LogisticsOrderCodeService {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    public static readonly string OrderCodePrefix = "SD";

    private ISessionFactory sessionFactory;

    private static readonly object LockObject = new object();

    public LogisticsOrderCodeService(ISessionFactory sessionFactory) {
        this.sessionFactory = sessionFactory;
    }

    public string GenerateNewCode(int customerId, string customerCheckinCode) {
        if (string.IsNullOrWhiteSpace(customerCheckinCode)) {
            throw new ValidationException($"客户#{customerId}入库校验号不能为空！");
        }

        lock (LockObject) // lock() 时，数据库事务应该是在内部，否则 lock() 后有时也没意义
        {
            int customerOrderSeq = 0;

            ISession? sessionLocal = null;
            ITransaction? transaction = null;
            try {
                sessionLocal = HibernateSessionFactory.OpenSession(sessionFactory);
                transaction = sessionLocal.BeginTransaction();

                // 改成序列的方式
                customerOrderSeq = sessionLocal.CreateSQLQuery($"CALL logistics_order_code_customer_next_code({customerId})").UniqueResult<int>();

                sessionLocal.Flush();
                transaction.Commit();
            }
            catch (Exception exp) {
                log.Error(exp);
                transaction?.Rollback();

                throw;
            }
            finally {
                sessionLocal?.Close();
            }

            return $"{OrderCodePrefix}{customerCheckinCode}{(customerOrderSeq + 1):#0000}";
        }
    }
}