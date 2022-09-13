using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace shunda.Domain.Pass;

public class UserLoginLogInfoMapping : ClassMapping<UserLoginLogInfo>
{
    public UserLoginLogInfoMapping()
    {
        Table("tbl_user_login_log");

        Lazy(true);
        DynamicInsert(false);
        DynamicUpdate(true);

        Id(
            x => x.Id,
            map => {
                map.Column("id");
                map.Generator(Generators.Native);
            }
        );

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
            x => x.IpAddress,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("ip_address");
                map.Length(200);
                map.NotNullable(true);
            }
        );

        Property(
            x => x.UserAgent,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("user_agent");
                map.Length(300);
                map.NotNullable(true);
            }
        );

        Property(
            x => x.LoginTime,
            map => {
                map.Type(NHibernateUtil.DateTime);
                map.Column("login_time");
                map.NotNullable(true);
            }
        );
    }
}