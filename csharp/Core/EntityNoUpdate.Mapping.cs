using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace shunda.Core;

public class EntityNoUpdateMapping<T> : ClassMapping<T> where T : EntityNoUpdate
{
    public EntityNoUpdateMapping()
    {
        Lazy(true);
        DynamicInsert(false);
        DynamicUpdate(true);
        Cache(x => x.Usage(CacheUsage.ReadWrite));

        Id(
            x => x.Id,
            map => {
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
    }
}