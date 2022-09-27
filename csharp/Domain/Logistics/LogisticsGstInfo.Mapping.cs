using NHibernate;
using shunda.Core;

namespace shunda.Domain.Logistics;

public class LogisticsGstInfoMapping : EntityMapping<LogisticsGstInfo>
{
    public LogisticsGstInfoMapping()
    {
        Table("tbl_logistics_gst");

        Property(
            x => x.Name,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("name");
                map.Length(200);
                map.NotNullable(true);
            }
        );

        Property(
            x => x.CalcBaseType,
            map => {
                map.Type<FeeCalcBaseTypeHibernateType>();
                map.Column("calc_base_type_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.StartAmount,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("start_amount");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.FeeRate,
            map => {
                map.Type(NHibernateUtil.Decimal);
                map.Column("fee_rate");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.OrderNum,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("order_num");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.Status,
            map => {
                map.Type<LogisticsGstStatusHibernateType>();
                map.Column("status_id");
                map.NotNullable(true);
            }
        );
    }
}