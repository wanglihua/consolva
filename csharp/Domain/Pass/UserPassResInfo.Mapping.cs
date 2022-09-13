using shunda.Core;
using NHibernate.Mapping.ByCode;

namespace shunda.Domain.Pass;

public class UserPassResInfoMapping : EntityNoUpdateMapping<UserPassResInfo>
{
    public UserPassResInfoMapping()
    {
        Table("tbl_user_pass_res");

        ManyToOne(
            x => x.User,
            map => {
                map.Class(typeof(UserInfo));
                map.Column("user_id");
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