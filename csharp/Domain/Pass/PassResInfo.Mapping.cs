using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace shunda.Domain.Pass;

public class PassResInfoMapping : ClassMapping<PassResInfo>
{
    public PassResInfoMapping()
    {
        Table("tbl_pass_res");

        Lazy(true);
        DynamicInsert(false);
        DynamicUpdate(true);

        Cache(x => x.Usage(CacheUsage.ReadWrite));

        Id(
            x => x.Id,
            map => {
                map.Column("id");
                map.Generator(Generators.Assigned);
            }
        );

        ManyToOne(
            x => x.Cate,
            map => {
                map.Class(typeof(PassResCateInfo));
                map.Column("cate_id");
                map.NotNullable(true);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        Property(
            x => x.Name,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("name");
                map.Length(100);
                map.NotNullable(true);
                map.Unique(true);
            }
        );

        Property(
            x => x.Description,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("description");
                map.Length(300);
            }
        );
    }
}