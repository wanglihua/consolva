using NHibernate;
using NHibernate.Mapping.ByCode;
using shunda.Core;

namespace shunda.Domain.Customer;

public class AddressCityInfoMapping : EntityNoCreateNoUpdateMapping<AddressCityInfo>
{
    public AddressCityInfoMapping()
    {
        Table("tbl_address_city");

        Id(
            x => x.Id,
            map => {
                map.Column("id");
                map.Generator(Generators.Assigned);
            }
        );

        ManyToOne(
            x => x.Province,
            map => {
                map.Class(typeof(AddressProvinceInfo));
                map.Column("province_id");
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