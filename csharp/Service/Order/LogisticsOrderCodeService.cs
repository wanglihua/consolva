using NHibernate;
using NLog;
using shunda.Core.Hibernate;
using shunda.Core.Service;

namespace shunda.Service.Order
{
    [Service(ServiceType.Scoped)]
    public class LogisticsOrderCodeService
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        public static readonly string OrderCodePrefix = "SD";

        private ISessionFactory sessionFactory;

        private static readonly object LockObject = new object();

        public LogisticsOrderCodeService(ISessionFactory sessionFactory)
        {
            this.sessionFactory = sessionFactory;
        }

        /*
            订单编号，可以改成：

            CN+客户ID数字部分（ID数字部分5位，不到5位的，前面就加0）+订单次数（4位，不足4位，前面加0）

            箱序订单编号+4位，不足部分前面加0。

            目前的箱序前面的箱号，系统内部显示，但标签上不再显示


            以客户ID：5001,第15个订单，举例：

            订单编号为：CN050010015；

            共3箱的话：CN0500100150001，CN0500100150002，CN0500100150003         
         */

        public string GenerateNewCode(int customerId)
        {
            lock (LockObject) // lock() 时，数据库事务应该是在内部，否则 lock() 后有时也没意义
            {
                int customerOrderSeq = 0;

                ISession? sessionLocal = null;
                ITransaction? transaction = null;
                try
                {
                    sessionLocal = HibernateSessionFactory.OpenSession(sessionFactory);
                    transaction = sessionLocal.BeginTransaction();

                    // 改成序列的方式
                    customerOrderSeq = sessionLocal.CreateSQLQuery($"CALL logistics_order_code_customer_next_code({customerId})").UniqueResult<int>();

                    sessionLocal.Flush();
                    transaction.Commit();
                }
                catch (Exception exp)
                {
                    log.Error(exp);
                    transaction?.Rollback();

                    throw;
                }
                finally
                {
                    sessionLocal?.Close();
                }

                return $"{OrderCodePrefix}{customerId:#00000}{(customerOrderSeq + 1):#0000}";
            }
        }
    }
}