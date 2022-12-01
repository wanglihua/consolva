using NHibernate.Mapping.ByCode;
using shunda.Core;

namespace shunda.Domain.Pass;

public class GroupMemberInfoMapping : EntityNoUpdateMapping<GroupMemberInfo>
{
    public GroupMemberInfoMapping()
    {
        Table("tbl_group_member");

        ManyToOne(
            x => x.Group,
            map => {
                map.Class(typeof(GroupInfo));
                map.Column("group_id");
                map.NotNullable(true);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );        

        ManyToOne(
            x => x.User,
            map => {
                map.Class(typeof(UserInfo));
                map.Column("user_id");
                map.NotNullable(true);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Join);
            }
        );
    }
}