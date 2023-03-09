using shunda.Core;
using NHibernate;
using NHibernate.Mapping.ByCode;

namespace shunda.Domain.Client;

public class AdvertBannerImageInfoMapping : EntityMapping<AdvertBannerImageInfo> {
    public AdvertBannerImageInfoMapping() {
        Table("tbl_advert_banner_image");

        ManyToOne(
            x => x.Banner,
            map => {
                map.Class(typeof(AdvertBannerInfo));
                map.Column("banner_id");
                map.NotNullable(true);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        Property(
            x => x.CountryId,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("country_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.ImageNum,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("image_num");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.StorePath,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("store_path");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.StoreName,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("store_name");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.LinkUrl,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("link_url");
                map.NotNullable(true);
            }
        );

    }
}