using shunda.Core;
using NHibernate;
using NHibernate.Mapping.ByCode;
using shunda.Domain.Pass;

namespace shunda.Domain.Client;

public class ClientAgreementInfoMapping : EntityMapping<ClientAgreementInfo> {
    public ClientAgreementInfoMapping() {
        Table("tbl_client_agreement");

        Property(
            x => x.Title,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("title");
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
            x => x.Country,
            map => {
                map.Type<CountryHibernateType>();
                map.Column("country_id");
                map.NotNullable(true);
            }
        );

        ManyToOne(
            x => x.Creator,
            map => {
                map.Class(typeof(UserInfo));
                map.Column("creator_id");
                map.NotNullable(true);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        Property(
            x => x.Status,
            map => {
                map.Type<ClientAgreementStatusHibernateType>();
                map.Column("status_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.PublishTime,
            map => {
                map.Type(NHibernateUtil.DateTime);
                map.Column("publish_time");
                map.NotNullable(false);
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