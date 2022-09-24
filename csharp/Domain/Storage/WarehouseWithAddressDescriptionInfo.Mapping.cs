using NHibernate;
using shunda.Core;

namespace shunda.Domain.Storage;

public class WarehouseWithAddressDescriptionInfoMapping : EntityMapping<WarehouseWithAddressDescriptionInfo>
{
    public WarehouseWithAddressDescriptionInfoMapping()
    {
        Table("tbl_warehouse");

        Property(
            x => x.Code,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("code");
                map.Length(100);
                map.NotNullable(true);
                map.Unique(true);
            }
        );

        Property(
            x => x.Name,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("name");
                map.Length(200);
                map.NotNullable(true);
                map.Unique(true);
            }
        );

        Property(
            x => x.Description,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("description");
                map.Length(500);
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Address,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("address");
                map.Length(500);
                map.NotNullable(true);
            }
        );

        Property(
            x => x.AddressDescription,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("address_description");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.OuterStatus,
            map => {
                map.Type<WarehouseStatusHibernateType>();
                map.Column("outer_status_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.InnerStatus,
            map => {
                map.Type<WarehouseStatusHibernateType>();
                map.Column("inner_status_id");
                map.NotNullable(true);
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