using NHibernate;
using shunda.Core;

namespace shunda.Domain.Customer;

public class CustomerPromotionImageDescInfoMapping : EntityMapping<CustomerPromotionImageSettingInfo> {
    public CustomerPromotionImageDescInfoMapping() {
        Table("tbl_customer_promotion_image_setting");

        Property(
            x => x.ClientShow,
            map => {
                map.Type(NHibernateUtil.Boolean);
                map.Column("client_show");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.ImageDesc,
            map => {
                map.Type(NHibernateUtil.StringClob);
                map.Column("image_desc");
                map.NotNullable(true);
            }
        );
    }
}