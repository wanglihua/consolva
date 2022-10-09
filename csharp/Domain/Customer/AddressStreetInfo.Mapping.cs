using shunda.Core;
using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace shunda.Domain.Customer;

public class AddressStreetInfoMapping : ClassMapping<AddressStreetInfo>
{
    public AddressStreetInfoMapping()
    {
        Table("tbl_address_street");

        Lazy(true);
        DynamicInsert(false);
        DynamicUpdate(true);
        Mutable(false); // 这些数据一般是不会通过程序改动的
        Cache(x => x.Usage(CacheUsage.ReadWrite));

        ManyToOne(
            x => x.Area,
            map => {
                map.Class(typeof(AddressAreaInfo));
                map.Column("area_id");
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