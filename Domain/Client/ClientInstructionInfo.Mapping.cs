using NHibernate;
using shunda.Core;
using shunda.Domain.Pass;

namespace shunda.Domain.Client;

public class ClientInstructionInfoMapping : EntityMapping<ClientInstructionInfo> {
    public ClientInstructionInfoMapping() {
        Table("tbl_client_instruction");

        Property(
            x => x.Type,
            map => {
                map.Type<ClientInstructionTypeHibernateType>();
                map.Column("type_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Title,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("title");
                map.Length(200);
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

        ManyToOne(
            x => x.Creator,
            map => {
                map.Class(typeof(UserInfo));
                map.Column("creator_id");
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
    }
}