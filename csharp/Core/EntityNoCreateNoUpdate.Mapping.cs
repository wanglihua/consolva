using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace shunda.Core;

public class EntityNoCreateNoUpdateMapping<T> : ClassMapping<T> where T : EntityNoCreateNoUpdate
{
    public EntityNoCreateNoUpdateMapping()
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
    }
}