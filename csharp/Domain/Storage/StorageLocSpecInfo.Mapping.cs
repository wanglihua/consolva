using NHibernate;
using shunda.Core;

namespace shunda.Domain.Storage;

public class StorageLocSpecInfoMapping : EntityMapping<StorageLocSpecInfo>
{
    public StorageLocSpecInfoMapping()
    {
        Table("tbl_storage_loc_spec");

        Property(
            x => x.ShelfType,
            map => {
                map.Type<ShelfTypeHibernateType>();
                map.Column("shelf_type_id");
                map.NotNullable(true);
                map.UniqueKey("ix_storage_loc_spec_type_id_code");
            }
        );

        Property(
            x => x.Code,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("code");
                map.Length(50);
                map.NotNullable(true);
                map.UniqueKey("ix_storage_loc_spec_type_id_code");
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
            x => x.Width,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("width");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Height,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("height");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Length,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("length");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Color,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("color");
                map.NotNullable(true);
            }
        );
    }
}