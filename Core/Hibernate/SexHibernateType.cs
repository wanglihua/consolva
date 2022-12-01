using System.Data;
using System.Data.Common;
using NHibernate;
using NHibernate.Engine;
using NHibernate.SqlTypes;

namespace shunda.Core.Hibernate;

#nullable disable

public class SexHibernateType : HibernateTypeBase
{
    public override Type ReturnedType
    {
        get { return typeof(SexType); }
    }

    public override SqlType[] SqlTypes
    {
        get
        {
            SqlType sqlType = new StringFixedLengthSqlType(1);
            return new SqlType[] { sqlType };
        }
    }

    public override object NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session, object owner)
    {
        object obj = NHibernateUtil.String.NullSafeGet(rs, names[0], session);
        string str = obj as string;

        if ((obj == null) || str == null || (str.Length == 0))
        {
            return null;
        }

        char sexCode = str[0];
        return SexType.GetSexByCode(sexCode);
    }

    public override void NullSafeSet(DbCommand cmd, object value, int index, ISessionImplementor session)
    {
        if (value == null)
        {
            ((IDataParameter)cmd.Parameters[index]).Value = DBNull.Value;
        }
        else
        {
            SexType sex = (SexType)value;
            ((IDataParameter)cmd.Parameters[index]).Value = sex.Code;
        }
    }
}