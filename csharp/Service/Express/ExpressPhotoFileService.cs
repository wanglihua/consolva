using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
public class ExpressPhotoFileService
{
    private static Logger log = LogManager.GetCurrentClassLogger();

    public static readonly string ImageUploadFolder = Path.Combine("wwwroot", "static", "upload", "images", "express");
    public static readonly string ImageUploadUrl = "/static/upload/images/express";

    private static readonly int MAX_WIDTH = 800;
    private static readonly int MAX_HEIGHT = 600;

    private IWebHostEnvironment webHostEnvironment;

    public ExpressPhotoFileService(IWebHostEnvironment webHostEnvironment)
    {
        this.webHostEnvironment = webHostEnvironment;
    }

    /// <summary>
    /// 保存指定的已上传文件
    /// </summary>
    /// <param name="imageFileData">图片数据</param>
    /// <returns>新的文件全名(以"/"开头)，存储路径(前后没有"/"和"\")，存储文件名((前后没有"/"和"\"))</returns>
    public Tuple<string, string, string> SaveImageFileData(byte[] imageFileData)
    {
        ImageFormat imageFormat = ImageFormat.Jpeg; // 约定是jpg
        string fileExt = ".jpg";
        string newFileName = Guid.NewGuid().ToString("N") + fileExt;
        string dayFolderName = DateTime.Now.ToString("yyyyMMdd");
        string fullUploadFolder = Path.Combine(webHostEnvironment.ContentRootPath, ImageUploadFolder, dayFolderName);
        string newFileFullName = Path.Combine(fullUploadFolder, newFileName);

        if (Directory.Exists(fullUploadFolder) == false)
        {
            Directory.CreateDirectory(fullUploadFolder);
        }

        MemoryStream memoryStream = new MemoryStream();
        memoryStream.Write(imageFileData);

        Bitmap originImage = new Bitmap(memoryStream);
        int originWidth = originImage.Width;
        int originHeight = originImage.Height;

        if (originWidth > MAX_WIDTH || originHeight > MAX_HEIGHT)
        {
            double ratioWidth = (double)MAX_WIDTH / (double)originWidth;
            double ratioHeight = (double)MAX_HEIGHT / (double)originHeight;
            double ratio = Math.Min(ratioWidth, ratioHeight);
            int newWidth = (int)(originWidth * ratio);
            int newHeight = (int)(originHeight * ratio);

            Bitmap newImage = new Bitmap(newWidth, newHeight);
            Graphics graphics = Graphics.FromImage(newImage);
            graphics.DrawImage(originImage, 0, 0, newWidth, newHeight);

            newImage.Save(newFileFullName, imageFormat);

            graphics.Dispose();
            newImage.Dispose();
        }
        else
        {
            Stream newFileStream = File.Create(newFileFullName);
            newFileStream.Write(imageFileData);
            newFileStream.Close();
        }

        originImage.Dispose();
        memoryStream.Dispose();

        return Tuple.Create($"{ImageUploadUrl}/{dayFolderName}/{newFileName}", dayFolderName, newFileName);
    }

    /// <summary>
    /// 保存指定的已上传文件
    /// </summary>
    /// <param name="formFile"></param>
    /// <returns>新的文件全名(以"/"开头)，存储路径(前后没有"/"和"\")，存储文件名((前后没有"/"和"\"))</returns>
    public Tuple<string, string, string> SaveImageFile(IFormFile formFile)
    {
        string fileExt = GetFileExt(formFile.FileName); // .jpg
        string newFileName = Guid.NewGuid().ToString("N") + fileExt;
        string monthFolderName = DateTime.Now.ToString("yyyyMM");
        string fullUploadFolder = Path.Combine(webHostEnvironment.ContentRootPath, ImageUploadFolder, monthFolderName);
        string newFileFullName = Path.Combine(fullUploadFolder, newFileName);

        if (Directory.Exists(fullUploadFolder) == false)
        {
            Directory.CreateDirectory(fullUploadFolder);
        }

        Bitmap originImage = new Bitmap(formFile.OpenReadStream());
        int originWidth = originImage.Width;
        int originHeight = originImage.Height;

        if (originWidth > MAX_WIDTH || originHeight > MAX_HEIGHT)
        {
            double ratioWidth = (double)MAX_WIDTH / (double)originWidth;
            double ratioHeight = (double)MAX_HEIGHT / (double)originHeight;
            double ratio = Math.Min(ratioWidth, ratioHeight);
            int newWidth = (int)(originWidth * ratio);
            int newHeight = (int)(originHeight * ratio);

            Bitmap newImage = new Bitmap(newWidth, newHeight);
            Graphics graphics = Graphics.FromImage(newImage);
            graphics.DrawImage(originImage, 0, 0, newWidth, newHeight);

            ImageFormat imageFormat = ImageFormat.Jpeg;
            if (fileExt == ".png")
            {
                imageFormat = ImageFormat.Png;
            }

            newImage.Save(newFileFullName, imageFormat);

            graphics.Dispose();
            newImage.Dispose();
        }
        else
        {
            Stream newFileStream = File.Create(newFileFullName);
            formFile.CopyTo(newFileStream);
            newFileStream.Close();
        }

        originImage.Dispose();

        return Tuple.Create($"{ImageUploadUrl}/{monthFolderName}/{newFileName}", monthFolderName, newFileName);
    }

    /// <summary>
    /// 获取文件的扩展名
    /// </summary>
    /// <param name="fileName">文件名称</param>
    /// <returns>文件扩展名，比如：".jpg', '.png' </returns>
    public string GetFileExt(string fileName)
    {
        string fileExt = new FileInfo(fileName).Extension; // .jpg
        return fileExt.ToLower();
    }

    #region 图片URL处理

    private enum PhotoRenderLocation
    {
        Site,
        App,
    }

    public string GetBigImageFullUrlForSite(ExpressPhotoInfo expressPhoto, OssSiteLocType siteLocType)
    {
        return GetBigImageFullUrl(expressPhoto, siteLocType, PhotoRenderLocation.Site);
    }

    public string GetBigImageFullUrlForApp(ExpressPhotoInfo expressPhoto, OssSiteLocType siteLocType)
    {
        return GetBigImageFullUrl(expressPhoto, siteLocType, PhotoRenderLocation.App);
    }

    private string GetBigImageFullUrl(ExpressPhotoInfo expressPhoto, OssSiteLocType siteLocType, PhotoRenderLocation renderLocation)
    {
        string storePath = expressPhoto.StorePath;
        string storeName = expressPhoto.StoreName;

        if (string.IsNullOrWhiteSpace(storePath) == false)
        {
            storePath = storePath.Replace('\\', '/');
        }

        if (string.IsNullOrWhiteSpace(storeName) == false)
        {
            storeName = storeName.Replace('\\', '/');
        }

        if (expressPhoto.StoreLoc == ExpressPhotoStoreLoc.Oss)
        {
            string watermarkSafeBase64 = "5pys5b-r6YCS5bey562-5pS277yM562-5pS25Lq677ya5r6z5ou86ZuG6L-Q"; // 本快递已签收，签收人：舜达集运

            if (expressPhoto.Type != ExpressPhotoType.SignIn)
            {
                watermarkSafeBase64 = GetUrlSafeBase64($"【舜达集运】【北京时间】{expressPhoto.CreateTime.ToString("yyyy-MM-dd HH:mm")} 拍摄");
            }

            if (siteLocType == OssSiteLocType.HongKong)
            {
                return $"{AliYunOssConfig.UrlPrefixHongKong}/{storePath}/{storeName}?x-oss-process=image/watermark,type_d3F5LXplbmhlaQ,size_20,text_{watermarkSafeBase64},color_FF0000,g_south,y_80";
            }
            else
            {
                return $"{AliYunOssConfig.UrlPrefixChina}/{storePath}/{storeName}?x-oss-process=image/watermark,type_d3F5LXplbmhlaQ,size_20,text_{watermarkSafeBase64},color_FF0000,g_south,y_80";
            }
        }
        else
        {
            if (renderLocation == PhotoRenderLocation.Site)
            {
                return $"{ImageUploadUrl}/{storePath}/{storeName}";
            }

            if (renderLocation == PhotoRenderLocation.App)
            {
                return $"{SiteConfig.SiteRoot}{ImageUploadUrl}/{storePath}/{storeName}";
            }

            return $"{ImageUploadUrl}/{storePath}/{storeName}";
        }
    }

    public string GetSmallImageFullUrlForSite(ExpressPhotoInfo expressPhoto, OssSiteLocType siteLocType)
    {
        return GetSmallImageFullUrl(expressPhoto, siteLocType, PhotoRenderLocation.Site);
    }

    public string GetSmallImageFullUrlForApp(ExpressPhotoInfo expressPhoto, OssSiteLocType siteLocType)
    {
        return GetSmallImageFullUrl(expressPhoto, siteLocType, PhotoRenderLocation.App);
    }

    private string GetSmallImageFullUrl(ExpressPhotoInfo expressPhoto, OssSiteLocType siteLocType, PhotoRenderLocation renderLocation)
    {
        string storePath = expressPhoto.StorePath;
        string storeName = expressPhoto.StoreName;

        if (string.IsNullOrWhiteSpace(storePath) == false)
        {
            storePath = storePath.Replace('\\', '/');
        }

        if (string.IsNullOrWhiteSpace(storeName) == false)
        {
            storeName = storeName.Replace('\\', '/');
        }

        if (expressPhoto.StoreLoc == ExpressPhotoStoreLoc.Oss)
        {
            if (siteLocType == OssSiteLocType.HongKong)
            {
                return $"{AliYunOssConfig.UrlPrefixHongKong}/{storePath}/{storeName}?x-oss-process=image/resize,h_100,m_lfit";
            }
            else
            {
                return $"{AliYunOssConfig.UrlPrefixChina}/{storePath}/{storeName}?x-oss-process=image/resize,h_100,m_lfit";
            }
        }
        else
        {
            if (renderLocation == PhotoRenderLocation.Site)
            {
                return $"{ImageUploadUrl}/{storePath}/{storeName}";
            }

            if (renderLocation == PhotoRenderLocation.App)
            {
                return $"{SiteConfig.SiteRoot}{ImageUploadUrl}/{storePath}/{storeName}";
            }

            return $"{ImageUploadUrl}/{storePath}/{storeName}";
        }
    }

    public string GetExtraSmallImageFullUrlForSite(ExpressPhotoInfo expressPhoto, OssSiteLocType siteLocType)
    {
        return GetExtraSmallImageFullUrl(expressPhoto, siteLocType, PhotoRenderLocation.Site);
    }

    public string GetExtraSmallImageFullUrlForApp(ExpressPhotoInfo expressPhoto, OssSiteLocType siteLocType)
    {
        return GetExtraSmallImageFullUrl(expressPhoto, siteLocType, PhotoRenderLocation.App);
    }

    private string GetExtraSmallImageFullUrl(ExpressPhotoInfo expressPhoto, OssSiteLocType siteLocType, PhotoRenderLocation renderLocation)
    {
        string storePath = expressPhoto.StorePath;
        string storeName = expressPhoto.StoreName;

        if (string.IsNullOrWhiteSpace(storePath) == false)
        {
            storePath = storePath.Replace('\\', '/');
        }

        if (string.IsNullOrWhiteSpace(storeName) == false)
        {
            storeName = storeName.Replace('\\', '/');
        }

        if (expressPhoto.StoreLoc == ExpressPhotoStoreLoc.Oss)
        {
            if (siteLocType == OssSiteLocType.HongKong)
            {
                return $"{AliYunOssConfig.UrlPrefixHongKong}/{storePath}/{storeName}?x-oss-process=image/resize,h_50,m_lfit";
            }
            else
            {
                return $"{AliYunOssConfig.UrlPrefixChina}/{storePath}/{storeName}?x-oss-process=image/resize,h_50,m_lfit";
            }
        }
        else
        {
            if (renderLocation == PhotoRenderLocation.Site)
            {
                return $"{ImageUploadUrl}/{storePath}/{storeName}";
            }

            if (renderLocation == PhotoRenderLocation.App)
            {
                return $"{SiteConfig.SiteRoot}{ImageUploadUrl}/{storePath}/{storeName}";
            }

            return $"{ImageUploadUrl}/{storePath}/{storeName}";
        }
    }

    private string GetUrlSafeBase64(string source)
    {
        string urlSafeBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(source));

        urlSafeBase64 = urlSafeBase64.Replace("+", "-");
        urlSafeBase64 = urlSafeBase64.Replace("/", "_");
        urlSafeBase64 = urlSafeBase64.TrimEnd('=');

        return urlSafeBase64;
    }

    #endregion

    public void DeleteFile(ExpressPhotoInfo expressPhoto)
    {
        try
        {
            string fullImageName = Path.Combine(webHostEnvironment.ContentRootPath, ImageUploadFolder, expressPhoto.StorePath, expressPhoto.StoreName);
            File.Delete(fullImageName);
        }
        catch (Exception exp)
        {
            log.Error(exp);
        }
    }

    /// <summary>
    /// 添加水印
    /// </summary>
    /// <param name="monthFolderName"></param>
    /// <param name="fileName"></param>
    /// <param name="photoType"></param>
    public void AddWatermark(string monthFolderName, string fileName, ExpressPhotoType photoType)
    {
        try
        {
            string imageFullFileName = Path.Combine(webHostEnvironment.ContentRootPath, ImageUploadFolder, monthFolderName, fileName);
            Bitmap oldImage = new Bitmap(imageFullFileName);

            int imageWidth = oldImage.Width;
            int imageHeight = oldImage.Height;

            Bitmap newImage = new Bitmap(imageWidth, imageHeight);

            Graphics graphics = Graphics.FromImage(newImage);
            graphics.DrawImage(oldImage, new Rectangle(0, 0, imageWidth, imageHeight), 0, 0, imageWidth, imageHeight, GraphicsUnit.Pixel);
            oldImage.Dispose();

            Font font = new Font("宋体", 14, FontStyle.Bold);
            Brush brush = new SolidBrush(Color.Red);

            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Center;

            int textRectHeight = 100;

            if (photoType == ExpressPhotoType.SignIn)
            {
                graphics.DrawString($"本快递已签收，签收人：舜达集运", font, brush, new RectangleF(0f, imageHeight - textRectHeight, imageWidth, imageHeight), format);
            }
            else
            {
                graphics.DrawString($"【舜达集运】【北京时间】{DateTime.Now.ToString("yyyy-MM-dd HH:mm")} 拍摄", font, brush, new RectangleF(0f, imageHeight - textRectHeight, imageWidth, imageHeight), format);
            }

            font.Dispose();
            brush.Dispose();

            newImage.Save(imageFullFileName, ImageFormat.Jpeg);

            graphics.Dispose();
            newImage.Dispose();
        }
        catch (Exception exp)
        {
            log.Error(exp);
        }
    }
}