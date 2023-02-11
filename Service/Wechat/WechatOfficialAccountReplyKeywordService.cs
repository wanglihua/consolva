using NHibernate;
using shunda.Core.Service;
using shunda.Domain.Wechat;

namespace shunda.Service.Wechat;

[Service(ServiceType.Scoped)]
public class WechatOfficialAccountReplyKeywordService {
    private ISession session;

    public WechatOfficialAccountReplyKeywordService(ISession session) {
        this.session = session;
    }

    /// <summary>
    /// 获取关键字回复
    /// </summary>
    /// <param name="keyword"></param>
    /// <returns></returns>
    public string GetKeywordReply(string keyword) {
        if (string.IsNullOrWhiteSpace(keyword)) {
            return string.Empty;
        }

        keyword = keyword.Trim();

        string replyContent = string.Empty;

        WechatOfficialAccountReplyKeywordInfo? keywordReply;

        if (keyword.Length < 2) {
            keywordReply = session.Query<WechatOfficialAccountReplyKeywordInfo>()
                .Where(e => e.Keyword == keyword)
                .Take(1)
                .SingleOrDefault();
        } else {
            keyword = keyword.Replace(",", ""); // 配置的关键字有些是用逗号隔开的， 这样替换一下就不会出现跨两个或者多个关键字的情况

            keywordReply = session.Query<WechatOfficialAccountReplyKeywordInfo>()
                .Where(e => e.Keyword.Contains(keyword))
                .Take(1)
                .SingleOrDefault();
        }

        if (keywordReply != null) {
            replyContent = keywordReply.Content;
        }

        return replyContent;
    }
}