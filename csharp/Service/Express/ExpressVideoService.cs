using shunda.Core.Config;
using shunda.Core.Service;
using shunda.Domain.Express;

namespace shunda.Service.Express;

[Service(ServiceType.Scoped)]
public class ExpressVideoService
{
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
}