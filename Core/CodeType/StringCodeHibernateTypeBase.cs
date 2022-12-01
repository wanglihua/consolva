using System.Data;
using System.Data.Common;
using shunda.Core.Hibernate;
using NHibernate;
using NHibernate.Engine;
using NHibernate.SqlTypes;

namespace shunda.Core.CodeType;

#nullable disable

public abstract class StringCodeHibernateTypeBase<T> : HibernateTypeBase where T : StringCodeTypeBase
{
    public override Type ReturnedType
    {
        get { return typeof(T); }
    }

    public override SqlType[] SqlTypes
    {
        get { return new SqlType[] { new SqlType(DbType.String), }; }
    }

    public override object NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session, object owner)
    {
        object obj = NHibernateUtil.String.NullSafeGet(rs, names[0], session);

        string code = "";
        try
        {
            code = (string)obj;
        }
        catch
        {
            return null;
        }

        return GetCodeTypeByCode(code);
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
            ((IDataParameter)cmd.Parameters[index]).Value = codeType.Code;
        }
    }

    protected abstract T GetCodeTypeByCode(string code);
}