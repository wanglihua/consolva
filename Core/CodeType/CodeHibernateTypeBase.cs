using System.Data;
using System.Data.Common;
using shunda.Core.Hibernate;
using NHibernate;
using NHibernate.Engine;
using NHibernate.SqlTypes;

namespace shunda.Core.CodeType;

#nullable disable

public abstract class CodeHibernateTypeBase<T> : HibernateTypeBase where T : CodeTypeBase
{
    public override Type ReturnedType
    {
        get { return typeof(T); }
    }

    public override SqlType[] SqlTypes
    {
        get { return new SqlType[] { new SqlType(DbType.Int32), }; }
    }

    public override object NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session, object owner)
    {
        object obj = NHibernateUtil.Int32.NullSafeGet(rs, names[0], session);

        int code = 0;
        try
        {
            code = (int)obj;
        }
        catch
        {
            return null;
        }

        return GetCodeTypeById(code);
    }

    public override void NullSafeSet(DbCommand cmd, object value, int index, ISessionImplementor session)
    {
        if (value == null)
        {
            ((IDataParameter)cmd.Parameters[index]).Value = DBNull.Value;
        }
        else
        {
            T codeType = (T)value;
            ((IDataParameter)cmd.Parameters[index]).Value = codeType.Id;
        }
    }

    protected abstract T GetCodeTypeById(int id);
}