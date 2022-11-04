using shunda.Core;
using NHibernate;
using NHibernate.Mapping.ByCode;

namespace shunda.Domain.Express;

public class ExpressRemarkPresetSelectInfoMapping : EntityMapping<ExpressRemarkPresetSelectInfo>
{
    public ExpressRemarkPresetSelectInfoMapping()
    {
        Table("tbl_express_remark_preset_select");

        Property(
            x => x.Type,
            map => {
                map.Type<ExpressRemarkTypeHibernateType>();
                map.Column("type_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Remark,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("remark");
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

    }
}