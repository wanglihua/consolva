using NHibernate;
using NHibernate.Mapping.ByCode;
using shunda.Core;

namespace shunda.Domain.Storage;

public class StorageLocInfoMapping : EntityMapping<StorageLocInfo>
{
    public StorageLocInfoMapping()
    {
        Table("tbl_storage_loc");

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
            x => x.RowNum,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("row_num");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.ColNum,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("col_num");
                map.NotNullable(true);
            }
        );

        ManyToOne(
            x => x.Spec,
            map => {
                map.Class(typeof(StorageLocSpecInfo));
                map.Column("spec_id");
                map.NotNullable(true);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        ManyToOne(
            x => x.Shelf,
            map => {
                map.Class(typeof(ShelfInfo));
                map.Column("shelf_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Status,
            map => {
                map.Type<StorageLocStatusHibernateType>();
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

        Property(
            x => x.OffShelfOrderNum,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("off_shelf_order_num");
                map.NotNullable(true);
            }
        );
    }
}