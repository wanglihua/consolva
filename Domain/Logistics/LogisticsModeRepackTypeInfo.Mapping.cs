using NHibernate.Mapping.ByCode;
using shunda.Core;

namespace shunda.Domain.Logistics;

public class LogisticsModeRepackTypeInfoMapping : EntityMapping<LogisticsModeRepackTypeInfo>
{
    public LogisticsModeRepackTypeInfoMapping()
    {
        Table("tbl_logistics_mode_repack_type");

        ManyToOne(
            x => x.LogisticsMode,
            map => {
                map.Class(typeof(LogisticsModeInfo));
                map.Column("logistics_mode_id");
                map.NotNullable(true);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        Property(
            x => x.RepackType,
            map => {
                map.Type<LogisticsRepackTypeHibernateType>();
                map.Column("repack_type_id");
                map.NotNullable(true);
            }
        );
    }
}