using shunda.Core;
using NHibernate;
using NHibernate.Mapping.ByCode;
using shunda.Domain.Pass;

namespace shunda.Domain.Client;

public class ClientNoticeInfoMapping : EntityMapping<ClientNoticeInfo> {
    public ClientNoticeInfoMapping() {
        Table("tbl_client_notice");

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

        ManyToOne(
            x => x.EditorUser,
            map => {
                map.Class(typeof(UserInfo));
                map.Column("editor_user_id");
                map.NotNullable(true);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );

        Property(
            x => x.ListTop,
            map => {
                map.Type(NHibernateUtil.Boolean);
                map.Column("list_top");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Important,
            map => {
                map.Type(NHibernateUtil.Boolean);
                map.Column("important");
                map.NotNullable(true);
            }
        );

    }
}