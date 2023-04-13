using shunda.Core;
using NHibernate;

namespace shunda.Domain.LogisticsTrack;

public class SeventeenTrackCarrierInfoMapping : EntityMapping<SeventeenTrackCarrierInfo> {
    public SeventeenTrackCarrierInfoMapping() {
        Table("tbl_seventeen_track_carrier");

        Property(
            x => x.CarrierKey,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("carrier_key");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.CountryCode,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("country_code");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.CountryIso,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("country_iso");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Email,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("email");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Tel,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("Tel");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Url,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("url");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.CarrierName,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("carrier_name");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.NameZhCn,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("name_zh_cn");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.NameZhHk,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("name_zh_hk");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.CarrierGroup,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("carrier_group");
                map.NotNullable(true);
            }
        );

    }
}