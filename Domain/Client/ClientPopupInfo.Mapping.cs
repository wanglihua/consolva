using shunda.Core;
using NHibernate;

namespace shunda.Domain.Client;

public class ClientPopupInfoMapping : EntityMapping<ClientPopupInfo> {
    public ClientPopupInfoMapping() {
        Table("tbl_client_popup");

        Property(
            x => x.ShouldPopup,
            map => {
                map.Type(NHibernateUtil.Boolean);
                map.Column("should_popup");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Content,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("content");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.EditTime,
            map => {
                map.Type(NHibernateUtil.DateTime);
                map.Column("edit_time");
                map.NotNullable(true);
            }
        );

    }
}