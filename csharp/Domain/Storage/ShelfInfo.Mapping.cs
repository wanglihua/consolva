using NHibernate;
using NHibernate.Mapping.ByCode;
using shunda.Core;

namespace shunda.Domain.Storage;

public class ShelfInfoMapping : EntityMapping<ShelfInfo>
{
    public ShelfInfoMapping()
    {
        Table("tbl_shelf");

        Property(
            x => x.Code,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("code");
                map.Length(50);
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Name,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("name");
                map.Length(255);
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Type,
            map => {
                map.Type<ShelfTypeHibernateType>();
                map.Column("type_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Cate,
            map => {
                map.Type<ShelfCateHibernateType>();
                map.Column("cate_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Status,
            map => {
                map.Type<ShelfStatusHibernateType>();
                map.Column("status_id");
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

        ManyToOne(
            x => x.Warehouse,
            map => {
                map.Class(typeof(WarehouseInfo));
                map.Column("warehouse_id");
                map.NotNullable(true);
                map.Fetch(FetchKind.Select);
                map.Cascade(Cascade.None);
            }
        );
    }
}