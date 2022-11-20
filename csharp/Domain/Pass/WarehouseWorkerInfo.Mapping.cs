using shunda.Core;
using NHibernate;
using NHibernate.Mapping.ByCode;
using shunda.Domain.Storage;

namespace shunda.Domain.Pass;

public class WarehouseWorkerInfoMapping : EntityMapping<WarehouseWorkerInfo>
{
    public WarehouseWorkerInfoMapping()
    {
        Table("tbl_warehouse_worker");

        Property(
            x => x.Type,
            map => {
                map.Type<WarehouseWorkerTypeHibernateType>();
                map.Column("type_id");
                map.NotNullable(true);
            }
        );

        ManyToOne(
            x => x.Warehouse,
            map => {
                map.Class(typeof(WarehouseInfo));
                map.Column("warehouse_id");
                map.NotNullable(true);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        ManyToOne(
            x => x.User,
            map => {
                map.Class(typeof(UserInfo));
                map.Column("user_id");
                map.NotNullable(true);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        Property(
            x => x.Description,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("description");
                map.NotNullable(true);
            }
        );

    }
}