using shunda.Core;
using NHibernate;

namespace shunda.Domain.Customer;

public class CustomerPromotionImageInfoMapping : EntityMapping<CustomerPromotionImageInfo> {
    public CustomerPromotionImageInfoMapping() {
        Table("tbl_customer_promotion_image");

        Property(
            x => x.ImageNum,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("image_num");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.ImageFileName,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("image_file_name");
                map.NotNullable(true);
            }
        );

    }
}