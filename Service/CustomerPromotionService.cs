using NHibernate;
using NLog;
using shunda.Core.Service;
using shunda.Domain.Customer;
using shunda.Domain.Wechat;

namespace shunda.Service;

/// <summary>
/// 客户推广 服务类
/// </summary>
[Service(ServiceType.Scoped)]
public class CustomerPromotionService {
    private static Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;

    public CustomerPromotionService(ISession session) {
        this.session = session;
    }

    /// <summary>
    /// 有新微信号关注公众号
    /// </summary>
    /// <param name="openId"></param>
    /// <param name="unionId"></param>
    /// <param name="sceneId"></param>
    public void NewFellow(string openId, string unionId, int sceneId) {
        var weiXinScene = session.Query<WechatSceneInfo>()
            .Where(e => e.Type == WechatSceneType.CustomerPromotion
                        && e.SceneId == sceneId)
            .Take(1)
            .SingleOrDefault();

        // log.Warn("CustomerPromotionService.NewFellow 00100");

        if (weiXinScene == null) {
            log.Error($"指定的客户推广SceneId：{sceneId} 不存在！");

            // log.Warn("CustomerPromotionService.NewFellow 00200");

            return;
        }

        // log.Warn("CustomerPromotionService.NewFellow 00300");

        int parentCustomerId = weiXinScene.RelatedId;
        CustomerInfo customerParent = session.Get<CustomerInfo>(parentCustomerId);

        if (customerParent == null) {
            // log.Warn("CustomerPromotionService.NewFellow 00400");
            return;
        }

        var promotionExist = session.Query<CustomerPromotionInfo>()
            .Where(e => e.OpenId == openId)
            .Take(1)
            .SingleOrDefault();
        if (promotionExist != null) {
            // 这是防止重复推广积分
            // log.Warn("CustomerPromotionService.NewFellow 00500");
            return;
        }

        var customerChild = session.Query<CustomerInfo>()
            .Where(e => e.WechatOfficialAccountOpenId == openId || e.WechatUnionId == unionId)
            .OrderByDescending(e => e.Id)
            .Take(1)
            .SingleOrDefault();

        CustomerPromotionInfo customerPromotion = new CustomerPromotionInfo {
            Parent = customerParent,
            Child = customerChild, // 字段可空，但实际现在一般都非空
            OpenId = openId,
        };

        session.Save(customerPromotion);
        session.Flush();
    }
}