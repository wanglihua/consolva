using shunda.Core;
using NHibernate;
using NHibernate.Mapping.ByCode;
using shunda.Domain.Pass;
using shunda.Domain.Storage;

namespace shunda.Domain.Express;

public class ExpressDumpInfoMapping : EntityMapping<ExpressDumpInfo>
{
    public ExpressDumpInfoMapping()
    {
        Table("tbl_express_dump");

        Property(
            x => x.Type,
            map => {
                map.Type<ExpressDumpTypeHibernateType>();
                map.Column("type_id");
                map.NotNullable(true);
            }
        );

        ManyToOne(
            x => x.Express,
            map => {
                map.Class(typeof(ExpressInfo));
                map.Column("express_id");
                map.NotNullable(true);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        Property(
            x => x.TrackingNo,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("tracking_no");
                map.NotNullable(true);
            }
        );

        ManyToOne(
            x => x.DumpUser,
            map => {
                map.Class(typeof(UserInfo));
                map.Column("dump_user_id");
                map.NotNullable(true);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        Property(
            x => x.DumpTime,
            map => {
                map.Type(NHibernateUtil.DateTime);
                map.Column("dump_time");
                map.NotNullable(true);
            }
        );

        ManyToOne(
            x => x.Shelf,
            map => {
                map.Class(typeof(ShelfInfo));
                map.Column("shelf_id");
                map.NotNullable(true);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        ManyToOne(
            x => x.StorageLoc,
            map => {
                map.Class(typeof(StorageLocInfo));
                map.Column("storage_loc_id");
                map.NotNullable(true);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        ManyToOne(
            x => x.StockCheckUser,
            map => {
                map.Class(typeof(UserInfo));
                map.Column("stock_check_user_id");
                map.NotNullable(false);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        Property(
            x => x.StockCheckTime,
            map => {
                map.Type(NHibernateUtil.DateTime);
                map.Column("stock_check_time");
                map.NotNullable(false);
            }
        );

    }
}