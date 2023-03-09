using System.Drawing;
using Microsoft.AspNetCore.Http;
using NLog;
using shunda.Core;
using shunda.Core.Config;
using shunda.Core.Service;
using shunda.Domain.Client;
using ISession = NHibernate.ISession;

namespace shunda.Service.Client;

/// <summary>
/// 广告横幅图片 服务类
/// </summary>
[Service(ServiceType.Scoped)]
public class AdvertBannerImageService {
    private static Logger log = LogManager.GetCurrentClassLogger();

    public static readonly string ImageUploadFolder = Path.Combine(SiteConfig.StaticFolder, "upload", "images", "advert", "banner");
    public static readonly string ImageUploadUrl = "/static/upload/images/advert/banner";

    private readonly ISession session;

    public AdvertBannerImageService(ISession session) {
        this.session = session;
    }

    /// <summary>
    /// 保存指定的已上传文件
    /// </summary>
    /// <param name="formFile"></param>
    /// <returns>新的文件全名(以"/"开头)，存储路径(前后没有"/"和"\")，存储文件名((前后没有"/"和"\"))</returns>
    public Tuple<string, string, string> SaveImageFile(IFormFile formFile) {
        string fileExt = GetFileExt(formFile.FileName); // .jpg
        string newFileName = Guid.NewGuid().ToString("N") + fileExt;
        string monthFolderName = DateTime.Now.ToString("yyyyMM");
        string fullUploadFolder = Path.Combine(ImageUploadFolder, monthFolderName);
        string newFileFullName = Path.Combine(fullUploadFolder, newFileName);

        if (Directory.Exists(fullUploadFolder) == false) {
            Directory.CreateDirectory(fullUploadFolder);
        }

        Bitmap originImage = new Bitmap(formFile.OpenReadStream());
        Stream newFileStream = File.Create(newFileFullName);
        formFile.CopyTo(newFileStream);
        newFileStream.Close();

        originImage.Dispose();

        return Tuple.Create($"/{ImageUploadUrl}/{monthFolderName}/{newFileName}", monthFolderName, newFileName);
    }

    /// <summary>
    /// 获取文件的扩展名
    /// </summary>
    /// <param name="fileName">文件名称</param>
    /// <returns>文件扩展名，比如：".jpg', '.png' </returns>
    public string GetFileExt(string fileName) {
        string fileExt = new FileInfo(fileName).Extension; // .jpg
        return fileExt.ToLower();
    }

    /// <summary>
    /// 删除图片磁盘文件
    /// </summary>
    /// <param name="advertBannerImage"></param>
    /// <exception cref="ValidationException"></exception>
    public void DeleteImageFile(AdvertBannerImageInfo advertBannerImage) {
        if (advertBannerImage == null) {
            throw new ValidationException("要删除的广告横幅图片文件不能为空！");
        }

        try {
            string fullImageName = Path.Combine(ImageUploadFolder, advertBannerImage.StorePath, advertBannerImage.StoreName);
            File.Delete(fullImageName);
        }
        catch (Exception exp) {
            log.Error(exp);
        }
    }

    public string GetImageFullUrl(AdvertBannerImageInfo advertBannerImage) {
        if (advertBannerImage == null) {
            return string.Empty;
        }

        // 这个不判断应该也没事，万一有的时候真的存储路径为空呢
        // if (string.IsNullOrWhiteSpace(advertBannerImage.StorePath))
        // {
        //     return String.Empty;
        // }

        if (string.IsNullOrWhiteSpace(advertBannerImage.StoreName)) {
            return string.Empty;
        }

        string storePath = advertBannerImage.StorePath;
        string storeName = advertBannerImage.StoreName;

        if (string.IsNullOrWhiteSpace(storePath) == false) {
            storePath = storePath.Replace('\\', '/');
        }

        if (string.IsNullOrWhiteSpace(storeName) == false) {
            storeName = storeName.Replace('\\', '/');
        }

        return $"/{ImageUploadUrl}/{storePath}/{storeName}";
    }
}