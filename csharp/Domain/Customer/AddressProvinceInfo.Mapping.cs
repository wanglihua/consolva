using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace shunda.Domain.Customer;

public class AddressProvinceInfoMapping : ClassMapping<AddressProvinceInfo>
{
    public AddressProvinceInfoMapping()
    {
        Table("tbl_address_province");

        Lazy(true);
        DynamicInsert(false);
        DynamicUpdate(true);
        Mutable(false); // 这些数据一般是不会通过程序改动的
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
                map.NotNullable(true);
            }
        );

        Property(
            x => x.ShortName,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("short_name");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Lng,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("lng");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Lat,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("lat");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Sort,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("sort");
                map.NotNullable(true);
            }
        );
    }
}