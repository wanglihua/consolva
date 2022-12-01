using shunda.Core;
using NHibernate;
using NHibernate.Mapping.ByCode;

namespace shunda.Domain.Pass;

public class UserInfoMapping : EntityMapping<UserInfo>
{
    public UserInfoMapping()
    {
        Table("tbl_user");

        Property(
            x => x.LoginName,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("login_name");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.UserName,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("user_name");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Password,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("password");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.LastLoginTime,
            map => {
                map.Type(NHibernateUtil.DateTime);
                map.Column("last_login_time");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Status,
            map => {
                map.Type<UserStatusHibernateType>();
                map.Column("status_id");
                map.NotNullable(true);
            }
        );

    }
}