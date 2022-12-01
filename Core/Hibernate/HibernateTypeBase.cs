using System.Data.Common;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;

namespace shunda.Core.Hibernate;

public abstract class HibernateTypeBase : IUserType
{
    public abstract Type ReturnedType { get; }

    public abstract SqlType[] SqlTypes { get; }

    public abstract object NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session, object owner);

    public abstract void NullSafeSet(DbCommand cmd, object value, int index, ISessionImplementor session);

    public object Replace(object original, object target, object owner)
    {
        return original;
    }

    public bool IsMutable
    {
        get { return false; }
    }

    public object DeepCopy(object value)
    {
        return value;
    }

    public object Assemble(object cached, object owner)
    {
        return cached;
    }

    public object Disassemble(object value)
    {
        return value;
    }

    public new bool Equals(object x, object y)
    {
        return object.Equals(x, y);
    }

    public int GetHashCode(object x)
    {
        if (x == null)
        {
            return 0;
        }

        return x.GetHashCode();
    }
}