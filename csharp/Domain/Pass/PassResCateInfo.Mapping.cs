using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace shunda.Domain.Pass;

public class PassResCateInfoMapping : ClassMapping<PassResCateInfo>
{
    public PassResCateInfoMapping()
    {
        Table("tbl_pass_res_cate");

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

        Property(
            x => x.Name,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("name");
                map.Length(100);
                map.NotNullable(true);
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

        Property(
            x => x.OrderNum,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("order_num");
                map.NotNullable(true);
            }
        );
    }
}