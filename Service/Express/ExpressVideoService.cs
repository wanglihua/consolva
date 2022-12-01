using shunda.Core;
using shunda.Core.Config;
using shunda.Core.Service;
using shunda.Domain.Express;
using ISession = NHibernate.ISession;

namespace shunda.Service.Express;

[Service(ServiceType.Scoped)]
public class ExpressVideoService
{
    private readonly ISession session;

    public ExpressVideoService(ISession session)
    {
        this.session = session;
    }

    public string GetVideoFullUrl(ExpressVideoInfo expressVideo, OssSiteLocType siteLocType)
    {
        string storePath = expressVideo.StorePath;
        string storeName = expressVideo.StoreName;

        if (string.IsNullOrWhiteSpace(storePath) == false)
        {
            storePath = storePath.Replace('\\', '/');
        }

        if (string.IsNullOrWhiteSpace(storeName) == false)
        {
            storeName = storeName.Replace('\\', '/');
        }

        if (siteLocType == OssSiteLocType.HongKong)
        {
            return $"{AliYunOssConfig.UrlPrefixHongKong}/{storePath}/{storeName}";
        }
        else
        {
            return $"{AliYunOssConfig.UrlPrefixChina}/{storePath}/{storeName}";
        }
    }

    /// <summary>
    /// 绑定快递单号到视频，添加和更新快递单时调用
    /// </summary>
    public void BindExpressVideo(int expressId)
    {
        ExpressInfo? express = session.Get<ExpressInfo>(expressId);
        if (express == null)
        {
            throw new ValidationException($"指定的快递单号#{expressId}不存在！");
        }

        string trackingNo = express.TrackingNo.Trim();

        IList<ExpressVideoInfo> expressVideoList = session.Query<ExpressVideoInfo>()
            .Where(e => e.TrackingNo == trackingNo)
            .ToList();

        foreach (ExpressVideoInfo expressVideo in expressVideoList)
        {
            expressVideo.Express = express;
            expressVideo.Customer = express.Customer; // 有可能为 null，也有可能不是
            expressVideo.Order = express.Order; // 有可能为 null，也有可能不是
            if (express.Order != null)
            {
                expressVideo.OrderCode = express.Order.Code ?? string.Empty;
            }
            else
            {
                expressVideo.OrderCode = string.Empty;
            }

            session.Update(expressVideo);
        }

        session.Flush();
    }
}