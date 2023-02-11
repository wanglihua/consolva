using NHibernate;
using NLog;
using shunda.Core.Service;
using shunda.Domain.Customer;
using shunda.Domain.Wechat;
using shunda.Service.Customer;

namespace shunda.Service.Wechat;

// todo 类名和文件名 改名
[Service(ServiceType.Scoped)]
public class WeiXinFollowService {
    private static Logger log = LogManager.GetCurrentClassLogger();

    private ISession session;
    private CustomerPromotionService customerPromotionService;

    public WeiXinFollowService(ISession session, CustomerPromotionService customerPromotionService) {
        this.session = session;
        this.customerPromotionService = customerPromotionService;
    }

    /// <summary>
    /// 新的微信关注
    /// </summary>
    /// <param name="openId">open id</param>
    /// <param name="nickName">微信昵称</param>
    /// <param name="sceneId">场景Id</param>
    public void NewFellow(string openId, string nickName, int? sceneId) {
        // 一个 openid 一般【只】对应一个 WechatOfficialAccountFollowerInfo，不会有多于两个的情况
        var follower = session.Query<WechatOfficialAccountFollowerInfo>()
            .Where(e => e.OpenId == openId)
            .Take(1)
            .SingleOrDefault();

        // log.Warn("WeiXinFollowService.NewFellow 00100");

        bool isNewFollow = false;

        // 重复关注的话，就确认设置能是已关注就行了，这是用户推广的逻辑也不会有
        if (follower == null) {
            follower = new WechatOfficialAccountFollowerInfo();
            follower.OpenId = openId;

            // 在这里设置最早关注时间，也可以用create_time来代替

            isNewFollow = true;
        }

        follower.NickName = nickName;
        follower.FollowStatus = WechatOfficialAccountFollowStatus.Followed;
        follower.FollowTime = DateTime.Now;

        var customer = session.Query<CustomerInfo>()
            .Where(e => e.WechatOfficialAccountOpenId == openId)
            .Take(1)
            .SingleOrDefault();

        if (customer != null) {
            follower.Customer = customer;
        }

        session.SaveOrUpdate(follower);
        session.Flush();

        // 无论是新的还是老的关注都要更新客户表的状态
        if (customer != null) {
            customer.WechatOfficialAccountFollowStatus = WechatOfficialAccountFollowStatus.Followed;
            customer.WechatOfficialAccountFollowTime = DateTime.Now;

            session.Update(customer);
            session.Flush();

            return;
        }

        // 如果是老的关注者，下面的逻辑就不做了
        if (isNewFollow == false) {
            return;
        }

        if (sceneId != null && sceneId.Value != 0) {
            // log.Warn($"WeiXinFollowService.NewFellow 00400, {sceneId.Value}");

            // 场景关注处理

            int sceneIdValue = sceneId.Value;
            var weiXinScene = session.Query<WechatSceneInfo>()
                .Where(e => e.SceneId == sceneIdValue)
                .Take(1)
                .SingleOrDefault();

            // log.Warn($"WeiXinFollowService.NewFellow 00500, {sceneId.Value}");

            // 客户推广逻辑
            if (weiXinScene != null && weiXinScene.Type == WechatSceneType.CustomerPromotion) {
                // log.Warn("WeiXinFollowService.NewFellow 00600");

                // 是客户推广的场景值
                customerPromotionService.NewFellow(openId, sceneIdValue);
            }
        }
    }

    /// <summary>
    /// 新微信关注，无场景
    /// </summary>
    /// <param name="openId">open id</param>
    /// <param name="nickName">微信昵称</param>
    public void NewFellow(string openId, string nickName) {
        NewFellow(openId, nickName, null);
    }

    /// <summary>
    /// 取消关注
    /// </summary>
    /// <param name="openId"></param>
    public void UnFollow(string openId) {
        var customer = session.Query<CustomerInfo>()
            .Where(e => e.WechatOfficialAccountOpenId == openId)
            .Take(1)
            .SingleOrDefault();


        // 检查 微信关注者 表的记录

        var follower = session.Query<WechatOfficialAccountFollowerInfo>()
            .Where(e => e.OpenId == openId)
            .Take(1)
            .SingleOrDefault();

        if (follower == null) {
            follower = new WechatOfficialAccountFollowerInfo();
            follower.OpenId = openId;
        }

        if (customer != null) {
            follower.Customer = customer;
        }

        follower.FollowStatus = WechatOfficialAccountFollowStatus.UnFollowed;
        follower.UnfollowTime = DateTime.Now;

        session.SaveOrUpdate(follower);
        session.Flush();

        if (customer != null) {
            customer.WechatOfficialAccountFollowStatus = WechatOfficialAccountFollowStatus.UnFollowed;
            customer.WechatOfficialAccountUnfollowTime = DateTime.Now;

            session.Update(customer);
            session.Flush();
        }
    }
}

/*
 
 原来澳拼的逻辑

1. 是否 9688之前的ID。如果是，本次扫专属二维码无效

2. 是否关注公众号的时间早于 本次扫专属二维码的时间。如果是，本次扫专属二维码无效

3. 扫专属二维码后，注册（登录）客户ID是否小于 本次扫专属二维码的客户ID。如果是，本次扫专属二维码无效

4. 是否为其他人的下线。如果是，本次扫专属二维码无效

5. 是否自己扫了自己的专属二维码。如果是，本次扫专属二维码无效

6. 下家注册会员时，使用的IP地址，与上家相同的，本次扫专属二维码无效

7. 下家注册会员时，使用的家庭地址，与上家相同的，本次扫专属二维码无效

8. 如果是在注册之后关注的, 也算成是无效的

有些在这里实现, 有些在微信注册与登录的时候的时候实现, 标注为无效, 主要是关注时有些无法确定是否无效

*/