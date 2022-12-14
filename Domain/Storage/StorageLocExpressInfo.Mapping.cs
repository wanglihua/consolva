using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace shunda.Domain.Storage;

public class StorageLocExpressInfoMapping : ClassMapping<StorageLocExpressInfo>
{
    public StorageLocExpressInfoMapping()
    {
        Lazy(true);
        DynamicInsert(false);
        DynamicUpdate(true);
        Cache(x => x.Usage(CacheUsage.ReadWrite));

        Table("tbl_storage_loc_express");

        ComposedId(ci => {
            ci.Property(
                x => x.ExpressId,
                map => {
                    map.Type(NHibernateUtil.Int32);
                    map.Column("express_id");
                    map.NotNullable(true);
                });

            ci.ManyToOne(
                x => x.StorageLoc,
                map => {
                    map.Class(typeof(StorageLocInfo));
                    map.Column("storage_loc_id");
                    map.NotNullable(true);
                    map.Cascade(Cascade.None);
                    map.Fetch(FetchKind.Select);
                }
            );
        });

        ManyToOne(
            x => x.Shelf,
            map => {
                map.Class(typeof(ShelfInfo));
                map.Column("shelf_id");
                map.NotNullable(true);
            }
        );
    }
}