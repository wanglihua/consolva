using Microsoft.AspNetCore.Mvc;
using NLog;
using Senparc.Weixin;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.User;
using shunda.Core.Hibernate;
using shunda.Core.Web;
using shunda.Domain.Customer;
using shunda.Domain.Wechat;
using shunda.Service.Wechat;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Wechat.Controllers;

#pragma warning disable 0618

[AdminAuth(AdminOnly = true)]
[WechatArea]
public class WechatOfficialAccountFollowerController : Controller {
    private static Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;
    private readonly WechatOfficialAccountAccessTokenService wechatOfficialAccountAccessTokenService;

    public WechatOfficialAccountFollowerController(ISession session, WechatOfficialAccountAccessTokenService wechatOfficialAccountAccessTokenService) {
        this.session = session;
        this.wechatOfficialAccountAccessTokenService = wechatOfficialAccountAccessTokenService;
    }

    [HttpPost]
    [Route("/admin/wechat/official/account/follower/download/users")]
    public IActionResult DownloadUsers() {
        IList<string> openIdList = new List<string>();

        var accessToken = wechatOfficialAccountAccessTokenService.GetAccessToken();

        string nextOpenId = string.Empty;

        OpenIdResultJson userListResult;
        do {
            userListResult = UserApi.Get(accessToken, nextOpenId);

            foreach (string openId in userListResult.data.openid) {
                openIdList.Add(openId);
            }

            nextOpenId = userListResult.next_openid;
        } while (userListResult.errcode == ReturnCode.请求成功
                 && string.IsNullOrWhiteSpace(nextOpenId) == false
                 && userListResult.count >= 10000); // 最多一次拉取 10000 个

        foreach (string openId in openIdList) {
            var userInfo = UserApi.Info(accessToken, openId);
            string unionId = userInfo.unionid;

            if (userInfo.errcode == ReturnCode.请求成功) {
                Transactional.Wrap(session, () => {
                    var follower = session.Query<WechatOfficialAccountFollowerInfo>()
                        .Where(e => e.OpenId == openId || e.UnionId == unionId)
                        .Take(1)
                        .SingleOrDefault();

                    if (follower == null) {
                        follower = new WechatOfficialAccountFollowerInfo();
                    }

                    var customer = session.Query<CustomerInfo>()
                        .Where(e => e.WechatOfficialAccountOpenId == openId || e.WechatUnionId == unionId)
                        .Take(1)
                        .SingleOrDefault();

                    follower.Customer = customer;
                    follower.OpenId = openId;
                    follower.NickName = userInfo.nickname;
                    follower.FollowTime = new DateTime(2023, 1, 1);
                    follower.FollowStatus = WechatOfficialAccountFollowStatus.Followed;
                    session.SaveOrUpdate(follower);

                    session.Flush();
                });
            }
        }

        return Json(new JsonResultData {
            Success = true,
            Message = "下载公众号关注用户列表成功！！",
        });
    }
}