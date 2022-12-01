using NHibernate;
using NHibernate.Mapping.ByCode;
using shunda.Core;
using shunda.Domain.Storage;

namespace shunda.Domain.Express;

public class AutoMeasureMachineInfoMapping : EntityMapping<AutoMeasureMachineInfo>
{
    public AutoMeasureMachineInfoMapping()
    {
        Table("tbl_auto_measure_machine");

        Property(
            x => x.Name,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("name");
                map.NotNullable(true);
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

        Property(
            x => x.AppKey,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("app_key");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Secret,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("secret");
                map.NotNullable(true);
            }
        );
    }
}