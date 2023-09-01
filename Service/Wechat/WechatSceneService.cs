using System.Drawing;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using NLog;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;
using shunda.Core;
using shunda.Core.Config;
using shunda.Core.Hibernate;
using shunda.Core.Service;
using shunda.Domain.Customer;
using shunda.Domain.Wechat;
using ISession = NHibernate.ISession;

namespace shunda.Service.Wechat;

#pragma warning disable SYSLIB0014

[Service(ServiceType.Scoped)]
public class WechatSceneService {
    private static Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;
    private readonly WechatOfficialAccountAccessTokenService wechatAccessTokenService;

    public WechatSceneService(
        ISession session,
        WechatOfficialAccountAccessTokenService wechatAccessTokenService
    ) {
        this.session = session;
        this.wechatAccessTokenService = wechatAccessTokenService;
    }

    /// <summary>
    /// 使用二维码图片，本地缓存的方式
    /// </summary>
    /// <param name="customerId"></param>
    /// <returns></returns>
    public Bitmap CreateOrGetCustomerPromotionSceneImage(int customerId) {
        CustomerInfo customer = session.Get<CustomerInfo>(customerId);

        if (customer == null) {
            throw new ValidationException("指定的客户Id不存在！");
        }

        int sceneId = CreateOrGetCustomerPromotionWechatSceneId(customerId);

        string qrCodeFileFullName = GetQrCodeLocalFileFullName(sceneId);

        if (File.Exists(qrCodeFileFullName) == false) {
            RequestAndSaveSceneImageFromWechat(sceneId);
        }

        return new Bitmap(qrCodeFileFullName);
    }

    public string CreateOrGetCustomerPromotionSceneImageUrl(int customerId) {
        CustomerInfo customer = session.Get<CustomerInfo>(customerId);

        if (customer == null) {
            throw new ValidationException("指定的客户Id不存在！");
        }

        int sceneId = CreateOrGetCustomerPromotionWechatSceneId(customerId);

        string qrCodeFileFullName = GetQrCodeLocalFileFullName(sceneId);

        if (File.Exists(qrCodeFileFullName) == false) {
            RequestAndSaveSceneImageFromWechat(sceneId);
        }

        return GetQrCodeLocalUrl(sceneId);
    }

    private static object wechatSceneIdLockObj = new object();

    /// <summary>
    /// 创建或者获取客户推广 微信场景信息，这里都是客户推广场景
    /// 返回的信息不为空
    /// </summary>
    /// <param name="customerId"></param>
    /// <returns>场景ID</returns>
    public int CreateOrGetCustomerPromotionWechatSceneId(int customerId) {
        lock (wechatSceneIdLockObj) // lock() 时，数据库事务应该是在内部，否则 lock() 后有时也没意义
        {
            return Transactional.Wrap(session, () => {
                var wechatScene = session.Query<WechatSceneInfo>()
                    .Where(e => e.Type == WechatSceneType.CustomerPromotion && e.RelatedId == customerId)
                    .Take(1)
                    .SingleOrDefault();

                if (wechatScene == null) {
                    int maxSceneId = 0;
                    try {
                        maxSceneId = session.Query<WechatSceneInfo>().Max(e => e.SceneId);
                    }
                    catch (InvalidOperationException exp) // 出现这个异常，应该是在第一个场景值(1)生成的时候
                    {
                        log.Error(exp);
                    }

                    wechatScene = new WechatSceneInfo {
                        Type = WechatSceneType.CustomerPromotion,
                        RelatedId = customerId,
                        SceneId = maxSceneId + 1
                    };

                    session.Save(wechatScene);
                    session.Flush();
                }

                return wechatScene.SceneId; // 千万不要写成 return wechatScene.Id; 这会是个严重错误
            });
        }
    }

    /// <summary>
    /// 生成客户推广场景二维码
    /// </summary>
    /// <param name="sceneId"></param>
    /// <returns>二维码URL</returns>
    /// <exception cref="ValidationException"></exception>
    private string GetCustomerPromotionWechatSceneQrCodeUrlFromWechat(int sceneId) {
        var accessToken = wechatAccessTokenService.GetAccessToken();
        var result = QrCodeApi.Create(accessToken, 0, sceneId, QrCode_ActionName.QR_LIMIT_SCENE); // QrCode_ActionName.QR_LIMIT_SCENE 永久二维码
        string url = QrCodeApi.GetShowQrCodeUrl(result.ticket);

        return url;
    }

    /// <summary>
    /// 获取并保存场景二维码
    /// </summary>
    /// <param name="sceneId">场景ID</param>
    private void RequestAndSaveSceneImageFromWechat(int sceneId) {
        string qrCodeFileFullName = GetQrCodeLocalFileFullName(sceneId);

        string qrCodeUrl = GetCustomerPromotionWechatSceneQrCodeUrlFromWechat(sceneId);
        // string qrCodeUrl = "https://mp.weixin.qq.com/cgi-bin/showqrcode?ticket=gQEl8jwAAAAAAAAAAS5odHRwOi8vd2VpeGluLnFxLmNvbS9xLzAyMWF2MzQ0Q3FlXzIxMDAwMHcwM2QAAgS84o1dAwQAAAAA";

        HttpWebRequest qrCodeRequest = (HttpWebRequest)WebRequest.Create(qrCodeUrl);
        WebResponse qrCodeResponse = qrCodeRequest.GetResponse();

        Stream qrCodeStream = qrCodeResponse.GetResponseStream();

        Bitmap qrCodeImage = new Bitmap(qrCodeStream);
        qrCodeImage.Save(qrCodeFileFullName);
    }

    private string GetQrCodeLocalFileFullName(int sceneId) {
        string fileName = sceneId.ToString();
        return Path.Combine(SiteConfig.StaticFolder, "upload", "images", "scene", fileName + ".jpg");
    }

    private string GetQrCodeLocalUrl(int sceneId) {
        string fileName = sceneId.ToString();
        return SiteConfig.SiteUrlBase + "/static/upload/images/scene/" + fileName + ".jpg";
    }
}