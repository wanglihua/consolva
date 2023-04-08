using System.Text;
using NLog;
using shunda.Core.Config;
using shunda.Core.Service;
using shunda.Domain.Express;

namespace shunda.Service.Express;

/// <summary>
/// 这个类大部分处理跟图片和文件有关的逻辑
/// 数据业务处理相关的放在 ExpressPhotoService 中
/// </summary>
[Service(ServiceType.Scoped)]
public class ExpressPhotoFileService {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    public ExpressPhotoFileService() {
    }

    #region 图片URL处理

    private enum PhotoRenderLocation {
        Site,
        App,
    }

    public string GetBigImageFullUrlForSite(ExpressPhotoInfo expressPhoto, OssSiteLocType siteLocType) {
        return GetBigImageFullUrl(expressPhoto, siteLocType, PhotoRenderLocation.Site);
    }

    public string GetBigImageFullUrlForApp(ExpressPhotoInfo expressPhoto, OssSiteLocType siteLocType) {
        return GetBigImageFullUrl(expressPhoto, siteLocType, PhotoRenderLocation.App);
    }

    private string GetBigImageFullUrl(ExpressPhotoInfo expressPhoto, OssSiteLocType siteLocType, PhotoRenderLocation renderLocation) {
        string storePath = expressPhoto.StorePath;
        string storeName = expressPhoto.StoreName;

        if (string.IsNullOrWhiteSpace(storePath) == false) {
            storePath = storePath.Replace('\\', '/');
        }

        if (string.IsNullOrWhiteSpace(storeName) == false) {
            storeName = storeName.Replace('\\', '/');
        }

        if (expressPhoto.StoreLoc == ExpressPhotoStoreLoc.Oss) {
            string watermarkSafeBase64 = GetUrlSafeBase64("本快递已签收，签收人：舜达全球"); // 本快递已签收，签收人：舜达集运

            if (expressPhoto.Type != ExpressPhotoType.Sign) {
                watermarkSafeBase64 = GetUrlSafeBase64($"【舜达全球】【北京时间】{expressPhoto.CreateTime.ToString("yyyy-MM-dd HH:mm")} 拍摄");
            }

            if (siteLocType == OssSiteLocType.HongKong) {
                return $"{AliYunOssConfig.UrlPrefixHongKong}/{storePath}/{storeName}?x-oss-process=image/watermark,type_d3F5LXplbmhlaQ,size_20,text_{watermarkSafeBase64},color_FF0000,g_south,y_80";
            } else {
                return $"{AliYunOssConfig.UrlPrefixChina}/{storePath}/{storeName}?x-oss-process=image/watermark,type_d3F5LXplbmhlaQ,size_20,text_{watermarkSafeBase64},color_FF0000,g_south,y_80";
            }
        }

        return "";
    }

    public string GetSmallImageFullUrlForSite(ExpressPhotoInfo expressPhoto, OssSiteLocType siteLocType) {
        return GetSmallImageFullUrl(expressPhoto, siteLocType, PhotoRenderLocation.Site);
    }

    public string GetSmallImageFullUrlForApp(ExpressPhotoInfo expressPhoto, OssSiteLocType siteLocType) {
        return GetSmallImageFullUrl(expressPhoto, siteLocType, PhotoRenderLocation.App);
    }

    private string GetSmallImageFullUrl(ExpressPhotoInfo expressPhoto, OssSiteLocType siteLocType, PhotoRenderLocation renderLocation) {
        string storePath = expressPhoto.StorePath;
        string storeName = expressPhoto.StoreName;

        if (string.IsNullOrWhiteSpace(storePath) == false) {
            storePath = storePath.Replace('\\', '/');
        }

        if (string.IsNullOrWhiteSpace(storeName) == false) {
            storeName = storeName.Replace('\\', '/');
        }

        if (expressPhoto.StoreLoc == ExpressPhotoStoreLoc.Oss) {
            if (siteLocType == OssSiteLocType.HongKong) {
                return $"{AliYunOssConfig.UrlPrefixHongKong}/{storePath}/{storeName}?x-oss-process=image/resize,h_100,m_lfit";
            } else {
                return $"{AliYunOssConfig.UrlPrefixChina}/{storePath}/{storeName}?x-oss-process=image/resize,h_100,m_lfit";
            }
        }

        return "";
    }

    public string GetExtraSmallImageFullUrlForSite(ExpressPhotoInfo expressPhoto, OssSiteLocType siteLocType) {
        return GetExtraSmallImageFullUrl(expressPhoto, siteLocType, PhotoRenderLocation.Site);
    }

    public string GetExtraSmallImageFullUrlForApp(ExpressPhotoInfo expressPhoto, OssSiteLocType siteLocType) {
        return GetExtraSmallImageFullUrl(expressPhoto, siteLocType, PhotoRenderLocation.App);
    }

    private string GetExtraSmallImageFullUrl(ExpressPhotoInfo expressPhoto, OssSiteLocType siteLocType, PhotoRenderLocation renderLocation) {
        string storePath = expressPhoto.StorePath;
        string storeName = expressPhoto.StoreName;

        if (string.IsNullOrWhiteSpace(storePath) == false) {
            storePath = storePath.Replace('\\', '/');
        }

        if (string.IsNullOrWhiteSpace(storeName) == false) {
            storeName = storeName.Replace('\\', '/');
        }

        if (expressPhoto.StoreLoc == ExpressPhotoStoreLoc.Oss) {
            if (siteLocType == OssSiteLocType.HongKong) {
                return $"{AliYunOssConfig.UrlPrefixHongKong}/{storePath}/{storeName}?x-oss-process=image/resize,h_50,m_lfit";
            } else {
                return $"{AliYunOssConfig.UrlPrefixChina}/{storePath}/{storeName}?x-oss-process=image/resize,h_50,m_lfit";
            }
        }

        return "";
    }

    private string GetUrlSafeBase64(string source) {
        string urlSafeBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(source));

        urlSafeBase64 = urlSafeBase64.Replace("+", "-");
        urlSafeBase64 = urlSafeBase64.Replace("/", "_");
        urlSafeBase64 = urlSafeBase64.TrimEnd('=');

        return urlSafeBase64;
    }

    #endregion
}