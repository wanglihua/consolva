using shunda.Core;
using NHibernate;

namespace shunda.Domain.Client;

public class AdvertBannerInfoMapping : EntityMapping<AdvertBannerInfo> {
    public AdvertBannerInfoMapping() {
        Table("tbl_advert_banner");

        Property(
            x => x.Loc,
            map => {
                map.Type<AdvertBannerLocHibernateType>();
                map.Column("loc_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Name,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("name");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.ClientShow,
            map => {
                map.Type(NHibernateUtil.Boolean);
                map.Column("client_show");
                map.NotNullable(true);
            }
        );

    }
}