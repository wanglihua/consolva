using NHibernate;
using NHibernate.Mapping.ByCode;
using shunda.Core;

namespace shunda.Domain.Pass;

public class AdminInfoMapping : EntityMapping<AdminInfo>
{
    public AdminInfoMapping()
    {
        Table("tbl_admin");

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

        Property(
            x => x.LoginName,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("login_name");
                map.Length(100);
                map.NotNullable(true);
            }
        );

        Property(
            x => x.UserName,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("user_name");
                map.Length(100);
                map.NotNullable(true);
            }
        );
    }
}