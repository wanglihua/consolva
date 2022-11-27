using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace shunda.Core;

public class EntityMapping<T> : ClassMapping<T> where T : Entity
{
    public EntityMapping()
    {
        Lazy(true);
        DynamicInsert(false);
        DynamicUpdate(true);
        Cache(x => x.Usage(CacheUsage.ReadWrite));

        Id(
            x => x.Id,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("id");
                map.Generator(Generators.Native);
            }
        );

        Property(
            x => x.CreateTime,
            map => {
                map.Type(NHibernateUtil.DateTime);
                map.Column("create_time");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.UpdateTime,
            map => {
                map.Type(NHibernateUtil.DateTime);
                map.Column("update_time");
                map.NotNullable(true);
            }
        );
    }
}