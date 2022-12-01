using NHibernate.Mapping.ByCode;
using shunda.Core;
using shunda.Domain.Pass;
using shunda.Domain.Storage;

namespace shunda.Domain.Express;

public class ExpressCheckinUserShelfSelectInfoMapping : EntityMapping<ExpressCheckinUserShelfSelectInfo>
{
    public ExpressCheckinUserShelfSelectInfoMapping()
    {
        Table("tbl_express_checkin_user_shelf_select");

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
            x => x.StorageLocSpec,
            map => {
                map.Class(typeof(StorageLocSpecInfo));
                map.Column("storage_loc_spec_id");
                map.NotNullable(true);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

    }
}