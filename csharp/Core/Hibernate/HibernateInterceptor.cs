using NHibernate;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NLog;

namespace shunda.Core.Hibernate;

public class HibernateInterceptor : EmptyInterceptor
{
    private static Logger log = LogManager.GetCurrentClassLogger();

    private static readonly int indexInitValue = -1;
    private static readonly string createTimePropertyName = nameof(Entity.CreateTime);
    private static readonly string updateTimePropertyName = nameof(Entity.UpdateTime);

#if DEBUG
    private int sqlNum = 1;
#endif

    public override bool OnSave(object entity, object id, object[] state, string[] propertyNames, IType[] types)
    {
        if (entity is Entity e)
        {
            // find CreateTime UpdateTime index
            int createTimeIndex = indexInitValue;
            int updateTimeIndex = indexInitValue;
            for (int index = 0; index < propertyNames.Length; index++)
            {
                string propertyName = propertyNames[index];
                if (propertyName == createTimePropertyName)
                {
                    createTimeIndex = index;
                }

                if (propertyName == updateTimePropertyName)
                {
                    updateTimeIndex = index;
                }

                if (createTimeIndex != indexInitValue && updateTimeIndex != indexInitValue)
                {
                    break;
                }
            }

            // do modify
            DateTime now = DateTime.Now;

            // 这里认为 OnSave 只在新建保存的时候调用
            e.CreateTime = now;
            e.UpdateTime = now;

            state[createTimeIndex] = now;
            state[updateTimeIndex] = now;

            return true;
        }

        if (entity is EntityNoUpdate eNoUpdate)
        {
            // find CreateTime index
            int createTimeIndex = indexInitValue;
            for (int index = 0; index < propertyNames.Length; index++)
            {
                string propertyName = propertyNames[index];
                if (propertyName == createTimePropertyName)
                {
                    createTimeIndex = index;
                    break;
                }
            }

            // do modify
            DateTime now = DateTime.Now;

            // 这里认为 OnSave 只在新建保存的时候调用
            eNoUpdate.CreateTime = now;
            state[createTimeIndex] = now;

            return true;
        }

        return false;
    }

    public override bool OnFlushDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames, IType[] types)
    {
        if (entity is Entity e)
        {
            // find UpdateTime index
            int updateTimeIndex = indexInitValue;
            for (int index = 0; index < propertyNames.Length; index++)
            {
                string propertyName = propertyNames[index];

                if (propertyName == updateTimePropertyName)
                {
                    updateTimeIndex = index;
                    break;
                }
            }

            // do modify
            DateTime now = DateTime.Now;

            e.UpdateTime = now;
            currentState[updateTimeIndex] = now;

            return true;
        }

        return false;
    }

    public override SqlString OnPrepareStatement(SqlString sql)
    {
#if DEBUG
        log.Trace($"SQL NO {sqlNum++}: ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
#endif

        return sql;
    }
}