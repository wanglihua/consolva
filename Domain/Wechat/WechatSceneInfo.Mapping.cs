using NHibernate;
using shunda.Core;

namespace shunda.Domain.Wechat;

public class WechatSceneInfoMapping : EntityMapping<WechatSceneInfo> {
    public WechatSceneInfoMapping() {
        Table("tbl_wechat_scene");

        Property(
            x => x.SceneId,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("scene_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.Type,
            map => {
                map.Type<WechatSceneTypeHibernateType>();
                map.Column("type_id");
                map.NotNullable(true);
            }
        );

        Property(
            x => x.RelatedId,
            map => {
                map.Type(NHibernateUtil.Int32);
                map.Column("related_id");
                map.NotNullable(true);
            }
        );
    }
}