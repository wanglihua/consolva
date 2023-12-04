using NHibernate;
using NLog;
using shunda.Core.Hibernate;
using shunda.Core.Service;
using ISession = NHibernate.ISession;

namespace shunda.Service.Express;

[Service(ServiceType.Scoped)]
public class CheckinTrackingNoService {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private static readonly object LockObject = new object();

    private ISessionFactory sessionFactory;

    public CheckinTrackingNoService(ISessionFactory sessionFactory) {
        this.sessionFactory = sessionFactory;
    }

    public string GetNextMainTrackinigNo() {
        lock (LockObject) // lock() 时，数据库事务应该是在内部，否则 lock() 后有时也没意义
        {
            int seq;

            string yearLabel;
            string monthLabel;

            ISession? sessionLocal = null;
            ITransaction? transaction = null;
            try {
                sessionLocal = HibernateSessionFactory.OpenSession(sessionFactory);
                transaction = sessionLocal.BeginTransaction();

                object[] seqObjs = sessionLocal.CreateSQLQuery("CALL checkin_tracking_no_next_seq()").UniqueResult<object[]>();
                if (seqObjs == null || seqObjs.Length != 3) {
                    throw new Exception("获取下一个入库跟踪号失败！");
                }

                int year = int.Parse(seqObjs[0].ToString() ?? string.Empty);
                int month = int.Parse(seqObjs[1].ToString() ?? string.Empty);
                seq = int.Parse(seqObjs[2].ToString() ?? string.Empty);

                yearLabel = GetYearLetter(year);
                monthLabel = GetMonthLetter(month);

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

            return $"SD{seq:#00000}{yearLabel}{monthLabel}";
        }
    }

    private string GetYearLetter(int year) {
        return ((char)((int)'A' + year - 2023)).ToString(); // 2023年 是 A
    }

    private string GetMonthLetter(int month) {
        return ((char)((int)'A' + month - 1)).ToString(); // 1月 是A
    }
}