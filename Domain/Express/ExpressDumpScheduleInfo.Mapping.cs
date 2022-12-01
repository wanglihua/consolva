using NHibernate.Mapping.ByCode;
using shunda.Core;
using shunda.Domain.Pass;

namespace shunda.Domain.Express;

public class ExpressDumpScheduleInfoMapping : EntityMapping<ExpressDumpScheduleInfo>
{
    public ExpressDumpScheduleInfoMapping()
    {
        Table("tbl_express_dump_schedule");

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
            x => x.DumpType,
            map => {
                map.Type<ExpressDumpTypeHibernateType>();
                map.Column("dump_type_id");
                map.NotNullable(true);
            }
        );

    }
}