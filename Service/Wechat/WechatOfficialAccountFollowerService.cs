using NHibernate;
using NLog;
using shunda.Core.Service;
using shunda.Domain.Customer;
using shunda.Domain.Wechat;
using shunda.Service.Coupon;
using shunda.Service.Customer;

namespace shunda.Service.Wechat;

[Service(ServiceType.Scoped)]
public class WechatOfficialAccountFollowerService {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;
    private readonly CustomerService customerService;
    private readonly CustomerPromotionService customerPromotionService;
    private readonly CouponFollowService couponFollowService;

    public WechatOfficialAccountFollowerService(ISession session,
        CustomerService customerService,
        CustomerPromotionService customerPromotionService,
        CouponFollowService couponFollowService
    ) {
        this.session = session;
        this.customerService = customerService;
        this.customerPromotionService = customerPromotionService;
        this.couponFollowService = couponFollowService;
    }

    /// <summary>
    /// 通过微信公众号关注记录中的 union id 获取 微信公众号 open id
    /// </summary>
    /// <param name="unionId">union id</param>
    /// <returns></returns>
    public string GetWechatOfficialAccountOpenId(string unionId) {
        var follower = session.Query<WechatOfficialAccountFollowerInfo>()
            .Where(e => e.UnionId == unionId)
            .Take(1)
            .SingleOrDefault();
        return follower?.OpenId ?? string.Empty;
    }

    /// <summary>
    /// 通过微信公众号关注记录中的 微信公众号 open id 获取 union id 
    /// </summary>
    /// <param name="wechatOfficialAccountOpenId">微信公众号open id</param>
    /// <returns></returns>
    public string GetUnionId(string wechatOfficialAccountOpenId) {
        var follower = session.Query<WechatOfficialAccountFollowerInfo>()
            .Where(e => e.OpenId == wechatOfficialAccountOpenId)
            .Take(1)
            .SingleOrDefault();
        return follower?.UnionId ?? string.Empty;
    }

    /// <summary>
    /// 新的微信关注
    /// </summary>
    /// <param name="openId">open id</param>
    /// <param name="unionId">union id</param>
    /// <param name="nickName">微信昵称</param>
    /// <param name="sceneId">场景Id</param>
    public void NewFollow(string openId, string unionId, string nickName, int sceneId) {
        // 一个 openid 一般【只】对应一个 WechatOfficialAccountFollowerInfo，不会有多于两个的情况
        var follower = session.Query<WechatOfficialAccountFollowerInfo>()
            .Where(e => e.OpenId == openId)
            .Take(1)
            .SingleOrDefault();

        // log.Warn("WechatFollowService.NewFellow 00100");

        bool isNewFollow = false;

        // 重复关注的话，就确认设置能是已关注就行了，这是用户推广的逻辑也不会有
        if (follower == null) {
            follower = new WechatOfficialAccountFollowerInfo();
            follower.OpenId = openId;

            isNewFollow = true;
        }

        follower.UnionId = unionId;
        follower.NickName = nickName;
        follower.FollowStatus = WechatOfficialAccountFollowStatus.Followed;
        follower.FollowTime = DateTime.Now;

        session.SaveOrUpdate(follower);
        session.Flush();

        var customer = session.Query<CustomerInfo>()
            .Where(e => e.WechatOfficialAccountOpenId == openId || e.WechatUnionId == unionId)
            .Take(1)
            .SingleOrDefault();

        // 无论是新的还是老的关注都要更新客户表的状态
        if (customer != null) {
            customer.WechatOfficialAccountOpenId = openId;
            customer.WechatUnionId = unionId;
            customer.WechatOfficialAccountFollowStatus = WechatOfficialAccountFollowStatus.Followed;
            customer.WechatOfficialAccountFollowTime = DateTime.Now;

            session.Update(customer);
            session.Flush();
        }

        if (customer == null) {
            customer = customerService.CreateCustomerByWechatOfficialAccount(follower);
        }

        // 如果是老的关注者，下面的逻辑就不做了
        if (isNewFollow == false) {
            return;
        }

        couponFollowService.NewFollow(customer.Id);

        // 通过从公众号下载的关注用户，这里的逻辑也不会走
        // todo 在这里可以加上公众号关注奖励等等的逻辑 

        // 首次启用，防止出错
        try {
            if (sceneId > 0) {
                // log.Warn($"WechatFollowService.NewFellow 00400, {sceneId.Value}");
                // 场景关注处理

                var weiXinScene = session.Query<WechatSceneInfo>()
                    .Where(e => e.SceneId == sceneId)
                    .Take(1)
                    .SingleOrDefault();

                // log.Warn($"WechatFollowService.NewFellow 00500, {sceneId.Value}");

                // 客户推广逻辑
                if (weiXinScene != null && weiXinScene.Type == WechatSceneType.CustomerPromotion) {
                    // log.Warn("WechatFollowService.NewFellow 00600");

                    // 是客户推广的场景值
                    customerPromotionService.NewFellow(openId, unionId, sceneId);
                }
            }
        }
        catch (Exception exp) {
            log.Error(exp);
        }
    }

    /// <summary>
    /// 新微信关注，无场景
    /// </summary>
    /// <param name="openId">open id</param>
    /// <param name="unionId">union id</param>
    /// <param name="nickName">微信昵称</param>
    public void NewFollow(string openId, string unionId, string nickName) {
        NewFollow(openId, unionId, nickName, 0);
    }

    /// <summary>
    /// 取消关注
    /// </summary>
    /// <param name="openId">open id，只有这个参数，不使用 union id</param>
    public void UnFollow(string openId) {
        var customer = session.Query<CustomerInfo>()
            .Where(e => e.WechatOfficialAccountOpenId == openId) // 只按 open id: WechatOfficialAccountOpenId 查询 
            .Take(1)
            .SingleOrDefault();

        // 检查 微信关注者 表的记录

        var follower = session.Query<WechatOfficialAccountFollowerInfo>()
            .Where(e => e.OpenId == openId) // 只按 open id: WechatOfficialAccountOpenId 查询 
            .Take(1)
            .SingleOrDefault();

        if (follower == null) {
            follower = new WechatOfficialAccountFollowerInfo();
            follower.OpenId = openId;
            follower.UnionId = string.Empty; // 只能先设置一个空的字符串了
        }

        follower.FollowStatus = WechatOfficialAccountFollowStatus.UnFollowed;
        follower.UnfollowTime = DateTime.Now;

        session.SaveOrUpdate(follower);
        session.Flush();

        if (customer != null) {
            // 这个 open id 可以置空也可以不置空。下次关注时还是会设置一个一样的 open id。这里选择置空。
            customer.WechatOfficialAccountOpenId = string.Empty; // 只设置 open id: WechatOfficialAccountOpenId
            customer.WechatOfficialAccountFollowStatus = WechatOfficialAccountFollowStatus.UnFollowed;
            customer.WechatOfficialAccountUnfollowTime = DateTime.Now;

            session.Update(customer);
            session.Flush();
        }
    }
}