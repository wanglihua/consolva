using NHibernate;

namespace shunda.Core.Hibernate;

public class Transactional
{
    public static void Wrap(ISession session, Action action)
    {
        bool transactionBegin = false;

        ITransaction? transaction = null;

        try
        {
            transaction = session.GetCurrentTransaction();
            if (transaction == null)
            {
                transaction = session.BeginTransaction();
                transactionBegin = true;
            }

            // 执行业务代码
            action();

            if (transactionBegin == true)
            {
                session.Flush();
                transaction.Commit();
            }
        }
        catch (Exception exp)
        {
            transaction?.Rollback();
#pragma warning disable CA2200
            throw exp;
        }
    }
}