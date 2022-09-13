using shunda.Core;
using NHibernate;

namespace shunda.Domain.Pass;

public class GroupInfoMapping : EntityMapping<GroupInfo>
{
    public GroupInfoMapping()
    {
        Table("tbl_group");

        Property(
            x => x.System,
            map => {
                map.Type<PassResSystemHibernateType>();
                map.Column("system_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Code,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("code");
                map.Length(50);
                map.NotNullable(true);
                map.Unique(true);
            }
        );

        Property(
            x => x.Name,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("name");
                map.Length(50);
                map.NotNullable(true);
                map.Unique(true);
            }
        );

        Property(
            x => x.Description,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("description");
                map.Length(200);
            }
        );
    }
}