using NHibernate;
using NLog;

namespace shunda.Core.Hibernate;

public class Transactional {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    public static void Wrap(ISession session, Action action) {
        bool transactionBegin = false;

        ITransaction? transaction = null;

        try {
            transaction = session.GetCurrentTransaction();
            if (transaction == null) {
                transaction = session.BeginTransaction();
                transactionBegin = true;
            }

            // 执行业务代码
            action();

            if (transactionBegin == true) {
                session.Flush();
                transaction.Commit();
            }
        }
        catch (Exception exp) {
            transaction?.Rollback();

            log.Error(exp);

            throw;
        }
    }
}