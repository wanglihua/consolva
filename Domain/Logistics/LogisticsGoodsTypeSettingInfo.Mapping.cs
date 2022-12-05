using shunda.Core;
using NHibernate;
using NHibernate.Mapping.ByCode;

namespace shunda.Domain.Logistics;

public class LogisticsGoodsTypeSettingInfoMapping : EntityMapping<LogisticsGoodsTypeSettingInfo>
{
    public LogisticsGoodsTypeSettingInfoMapping()
    {
        Table("tbl_logistics_goods_type_setting");

        Property(
            x => x.Type,
            map => {
                map.Type<LogisticsGoodsTypeHibernateType>();
                map.Column("type_id");
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

        Property(
            x => x.SampleGoods,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("sample_goods");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Status,
            map => {
                map.Type<LogisticsGoodsTypeStatusHibernateType>();
                map.Column("status_id");
                map.NotNullable(true);
            }
        );

    }
}