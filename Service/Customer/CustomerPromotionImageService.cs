using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.AspNetCore.Http;
using NLog;
using shunda.Core;
using shunda.Core.Config;
using shunda.Core.Service;
using shunda.Domain.Customer;
using shunda.Service.Wechat;
using ISession = NHibernate.ISession;

namespace shunda.Service.Customer;

/// <summary>
/// 客户推广图片 服务类
/// </summary>
[Service(ServiceType.Scoped)]
public class CustomerPromotionImageService {
    private static Logger log = LogManager.GetCurrentClassLogger();

    public static string ImageUploadFolder = Path.Combine(SiteConfig.StaticFolder, "upload", "images", "promotion");
    public static string ImageUploadUrl = "/static/upload/images/promotion/";

    private readonly ISession session;
    private readonly WechatSceneService wechatSceneService;

    public CustomerPromotionImageService(
        ISession session,
        WechatSceneService wechatSceneService
    ) {
        this.session = session;
        this.wechatSceneService = wechatSceneService;
    }

    /// <summary>
    /// 保存指定的已上传文件
    /// </summary>
    /// <param name="formFile"></param>
    /// <returns>新的文件名</returns>
    public string SaveImageFile(IFormFile formFile) {
        string fileExt = new FileInfo(formFile.FileName).Extension; // .jpg
        string newFileName = Guid.NewGuid().ToString("N") + fileExt;
        string newFileFullName = Path.Combine(ImageUploadFolder, newFileName);

        if (Directory.Exists(ImageUploadFolder) == false) {
            Directory.CreateDirectory(ImageUploadFolder);
        }

        Stream newFileStream = File.Create(newFileFullName);
        formFile.CopyTo(newFileStream);
        newFileStream.Close();

        // return ImageUploadFolder.Replace("\\", "/") + "/" + newFileName;
        return newFileName;
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
    /// 删除指定的已上传文件，文件名不包含 ImageUploadFolder 部分
    /// </summary>
    /// <param name="imageFileName"></param>
    public void DeleteImageFile(string imageFileName) {
        string fullFileName = GetFullImageFileName(imageFileName);
        if (File.Exists(fullFileName) == false) {
            return;
        }

        File.Delete(fullFileName);
    }

    public string GetFullImageUrl(string imageFileName) {
        return ImageUploadUrl + imageFileName;
    }

    public string GetFullImageFileName(string imageFileName) {
        return Path.Combine(ImageUploadFolder, imageFileName);
    }

    #region 图片生成处理

    // 这里的有些数字跟上传图片的二维码摆放位置有关系

    public byte[] GetPromotionImageContentByScale(int customerId, int imageNum, decimal scale) {
        if (scale <= 0) {
            throw new ValidationException("推广图片缩放比例参数非法!");
        }

        var customerPromotionImage = session.Query<CustomerPromotionImageInfo>()
            .Where(e => e.ImageNum == imageNum)
            .Take(1)
            .SingleOrDefault();
        if (customerPromotionImage == null) {
            throw new ValidationException($"指定的推广图片序号{imageNum}不存在！");
        }

        Bitmap qrCodeImage = wechatSceneService.CreateOrGetCustomerPromotionSceneImage(customerId);

        Bitmap originalImage = new Bitmap(GetFullImageFileName(customerPromotionImage.ImageFileName));

        Bitmap image;
        if (scale == 1.0m) {
            image = originalImage;
        } else {
            image = new Bitmap(originalImage, new Size((int)(originalImage.Width * scale), (int)(originalImage.Height * scale)));
        }

        // Bitmap image = new Bitmap("z:/1.jpg");
        Graphics graphics = Graphics.FromImage(image);

        graphics.DrawImage(qrCodeImage, (int)(680 * scale), (int)(995 * scale), (int)(120 * scale), (int)(120 * scale));

        MemoryStream memoryStream = new MemoryStream();
        image.Save(memoryStream, ImageFormat.Jpeg);

        graphics.Dispose();
        image.Dispose();

        byte[] bytes = memoryStream.GetBuffer();
        memoryStream.Close();

        return bytes;
    }

    public byte[] GetPromotionImageContentByWidth(int customerId, int imageNum, int width) {
        if (width <= 0) {
            throw new ValidationException("推广图片高度参数非法!");
        }

        var customerPromotionImage = session.Query<CustomerPromotionImageInfo>()
            .Where(e => e.ImageNum == imageNum)
            .Take(1)
            .SingleOrDefault();
        if (customerPromotionImage == null) {
            throw new ValidationException($"指定的推广图片序号{imageNum}不存在！");
        }

        Bitmap qrCodeImage = wechatSceneService.CreateOrGetCustomerPromotionSceneImage(customerId);

        Bitmap originalImage = new Bitmap(GetFullImageFileName(customerPromotionImage.ImageFileName));

        decimal scale = (decimal)width / (decimal)(originalImage.Width);

        Bitmap image;
        decimal actualScale = 1.0m;
        if (0.95m <= scale && scale <= 1.05m) // 写成是一个范围，在这个范围内不缩放，因为实际计算值很少会是正好 1.0 的
        {
            actualScale = 1.0m;
            image = originalImage;
        } else {
            actualScale = scale;
            image = new Bitmap(originalImage, new Size((int)(originalImage.Width * actualScale), (int)(originalImage.Height * actualScale)));
        }

        // Bitmap image = new Bitmap("z:/1.jpg");
        Graphics graphics = Graphics.FromImage(image);

        graphics.DrawImage(qrCodeImage, (int)(680 * actualScale), (int)(995 * actualScale), (int)(120 * actualScale), (int)(120 * actualScale));

        MemoryStream memoryStream = new MemoryStream();
        image.Save(memoryStream, ImageFormat.Jpeg);

        graphics.Dispose();
        image.Dispose();

        byte[] bytes = memoryStream.GetBuffer();
        memoryStream.Close();

        return bytes;
    }

    public byte[] GetPromotionImageContentByHeight(int customerId, int imageNum, int height) {
        if (height <= 0) {
            throw new ValidationException("推广图片高度参数非法!");
        }

        var customerPromotionImage = session.Query<CustomerPromotionImageInfo>()
            .Where(e => e.ImageNum == imageNum)
            .Take(1)
            .SingleOrDefault();
        if (customerPromotionImage == null) {
            throw new ValidationException($"指定的推广图片序号{imageNum}不存在！");
        }

        Bitmap qrCodeImage = wechatSceneService.CreateOrGetCustomerPromotionSceneImage(customerId);

        Bitmap originalImage = new Bitmap(GetFullImageFileName(customerPromotionImage.ImageFileName));

        decimal scale = (decimal)height / (decimal)(originalImage.Height);

        Bitmap image;
        decimal actualScale = 1.0m;
        if (0.95m <= scale && scale <= 1.05m) // 写成是一个范围，在这个范围内不缩放，因为实际计算值很少会是正好 1.0 的
        {
            actualScale = 1.0m;
            image = originalImage;
        } else {
            actualScale = scale;
            image = new Bitmap(originalImage, new Size((int)(originalImage.Width * actualScale), (int)(originalImage.Height * actualScale)));
        }

        // Bitmap image = new Bitmap("z:/1.jpg");
        Graphics graphics = Graphics.FromImage(image);

        graphics.DrawImage(qrCodeImage, (int)(680 * actualScale), (int)(995 * actualScale), (int)(120 * actualScale), (int)(120 * actualScale));

        MemoryStream memoryStream = new MemoryStream();
        image.Save(memoryStream, ImageFormat.Jpeg);

        graphics.Dispose();
        image.Dispose();

        byte[] bytes = memoryStream.GetBuffer();
        memoryStream.Close();

        return bytes;
    }

    #endregion
}