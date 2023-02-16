using NHibernate;
using shunda.Core;

namespace shunda.Domain.Wechat;

public class WechatOfficialAccountFollowerInfoMapping : EntityMapping<WechatOfficialAccountFollowerInfo> {
    public WechatOfficialAccountFollowerInfoMapping() {
        Table("tbl_wechat_official_account_follower");

        Property(
            x => x.NickName,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("nick_name");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.OpenId,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("open_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.UnionId,
            map => {
                map.Type(NHibernateUtil.String);
                map.Column("union_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.FollowStatus,
            map => {
                map.Type<WechatOfficialAccountFollowStatusHibernateType>();
                map.Column("follow_status_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.FollowTime,
            map => {
                map.Type(NHibernateUtil.DateTime);
                map.Column("follow_time");
                map.NotNullable(false);
            }
        );

        Property(
            x => x.UnfollowTime,
            map => {
                map.Type(NHibernateUtil.DateTime);
                map.Column("unfollow_time");
                map.NotNullable(false);
            }
        );
    }
}