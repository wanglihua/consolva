using NHibernate;
using shunda.Core.Service;
using shunda.Domain.Wechat;

namespace shunda.Service.Wechat;

[Service(ServiceType.Scoped)]
public class WechatOfficialAccountReplyService {
    private ISession session;

    public WechatOfficialAccountReplyService(ISession session) {
        this.session = session;
    }

    /// <summary>
    /// 获取微信缺省回复
    /// </summary>
    /// <param name="replyType"></param>
    /// <returns></returns>
    public string GetReply(WechatOfficialAccountReplyType replyType) {
        var reply = session.Query<WechatOfficialAccountReplyInfo>()
            .Where(e => e.Type == replyType)
            .Take(1)
            .SingleOrDefault();

        string replyContent = reply?.Content?? string.Empty;

        if (string.IsNullOrWhiteSpace(replyContent)) {
            if (replyType == WechatOfficialAccountReplyType.FollowReply) {
                replyContent = GetDefaultFollowReply();
            }

            if (replyType == WechatOfficialAccountReplyType.TextMessageReply) {
                replyContent = GetDefaultTextMessageReply();
            }
        }

        return replyContent;
    }

    /// <summary>
    /// 获取缺省关注回复
    /// </summary>
    /// <returns></returns>
    private string GetDefaultFollowReply() {
        return @"欢迎关注舜达集运！";
    }

    /// <summary>
    /// 获取缺省文字消息回复
    /// </summary>
    /// <returns></returns>
    private string GetDefaultTextMessageReply() {
        return @"请使用应用内客户功能！";
    }
}