using shunda.Core;
using NHibernate.Mapping.ByCode;

namespace shunda.Domain.Pass;

public class GroupPassResInfoMapping : EntityNoUpdateMapping<GroupPassResInfo>
{
    public GroupPassResInfoMapping()
    {
        Table("tbl_group_pass_res");

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
            x => x.PassRes,
            map => {
                map.Class(typeof(PassResInfo));
                map.Column("pass_res_id");
                map.NotNullable(true);
                map.Cascade(Cascade.None);
                map.Fetch(FetchKind.Select);
            }
        );
    }
}