using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using NHibernate;
using NHibernate.Criterion;
using NLog;
using shunda.Core.Config;
using shunda.Core.Service;
using shunda.Domain.Express;
using shunda.Domain.Logistics;
using shunda.Service.Express;
using ISession = NHibernate.ISession;

namespace shunda.Service.Logistics;

[Service(ServiceType.Scoped)]
public class LogisticsGoodsImageService {
    private static Logger log = LogManager.GetCurrentClassLogger();

    public static string ImageUploadFolder = Path.Combine("static", "upload", "images", "goods");
    public static string ImageUploadUrl = "/static/upload/images/goods";

    private static readonly int MAX_WIDTH = 1024;
    private static readonly int MAX_HEIGHT = 768;

    private readonly IWebHostEnvironment webHostEnvironment;
    private readonly ISession session;

    public LogisticsGoodsImageService(
        IWebHostEnvironment webHostEnvironment,
        ISession session) {
        this.webHostEnvironment = webHostEnvironment;
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
        string subFolder = DateTime.Now.ToString("yyyyMM");
        string fullUploadFolder = Path.Combine(webHostEnvironment.WebRootPath, ImageUploadFolder, subFolder);
        string newFileFullName = Path.Combine(fullUploadFolder, newFileName);

        if (Directory.Exists(fullUploadFolder) == false) {
            Directory.CreateDirectory(fullUploadFolder);
        }

        Bitmap originImage = new Bitmap(formFile.OpenReadStream());
        int originWidth = originImage.Width;
        int originHeight = originImage.Height;

        if (originWidth > MAX_WIDTH || originHeight > MAX_HEIGHT) {
            double ratioWidth = (double)MAX_WIDTH / (double)originWidth;
            double ratioHeight = (double)MAX_HEIGHT / (double)originHeight;
            double ratio = Math.Min(ratioWidth, ratioHeight);
            int newWidth = (int)(originWidth * ratio);
            int newHeight = (int)(originHeight * ratio);

            Bitmap newImage = new Bitmap(newWidth, newHeight);
            Graphics graphics = Graphics.FromImage(newImage);
            graphics.DrawImage(originImage, 0, 0, newWidth, newHeight);

            ImageFormat imageFormat = ImageFormat.Jpeg;
            if (fileExt == ".png") {
                imageFormat = ImageFormat.Png;
            }

            newImage.Save(newFileFullName, imageFormat);

            graphics.Dispose();
            newImage.Dispose();
        } else {
            Stream newFileStream = File.Create(newFileFullName);
            formFile.CopyTo(newFileStream);
            newFileStream.Close();
        }

        originImage.Dispose();

        return Tuple.Create($"{ImageUploadUrl}/{subFolder}/{newFileName}", subFolder, newFileName);
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

    public string GetFullImageUrl(string subPath, string fileName) {
        return $"{ImageUploadUrl}/{subPath}/{fileName}";
    }

    public void DeleteFile(LogisticsGoodsImageInfo goodsImage) {
        try {
            string fullImageName = Path.Combine(webHostEnvironment.WebRootPath, ImageUploadFolder, goodsImage.StorePath, goodsImage.StoreName);
            File.Delete(fullImageName);
        }
        catch (Exception exp) {
            log.Error(exp);
        }
    }

    #region 关联订单图片的方式逻辑

    /// <summary>
    /// 物品是否至少有一张对应的图片
    /// </summary>
    /// <param name="goodsId"></param>
    /// <returns></returns>
    public bool HasGoodsImage(int goodsId) {
        var goodsImageId = session.Query<LogisticsGoodsImageInfo>()
            .Where(e => e.Goods.Id == goodsId)
            .Select(e => e.Id)
            .Take(1)
            .SingleOrDefault();
        if (goodsImageId != 0) {
            return true;
        }

        LogisticsGoodsInfo? logisticsGoods = session.Get<LogisticsGoodsInfo>(goodsId);
        if (logisticsGoods == null) {
            return false;
        }

        if (string.IsNullOrWhiteSpace(logisticsGoods.CreateExpressTrackingNo) == false) {
            string createExpressTrackingNo = logisticsGoods.CreateExpressTrackingNo.Trim();
            int expressPhotoId = session.Query<ExpressPhotoInfo>()
                .Where(e => e.TrackingNo == createExpressTrackingNo)
                .Select(e => e.Id)
                .Take(1)
                .SingleOrDefault();

            if (expressPhotoId != 0) {
                return true;
            }
        }

        ExpressPhotoInfo? ep = null;
        ExpressGoodsInfo? eg = null;
        int epId = session.QueryOver<ExpressPhotoInfo>(() => ep!)
            .JoinEntityQueryOver(() => eg, () => ep!.Express!.Id == eg!.Express.Id) // 暂时用这个办法
            .Select(e => ep!.Id)
            .Where(() => eg!.Goods.Id == goodsId)
            .Take(1)
            .SingleOrDefault<int>();

        if (epId != 0) {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 获取创建单号，关联图片
    /// 这个方法是取快递单号图片里面的图片，而不是后来上传的
    /// </summary>
    /// <param name="trackingNoList"></param>
    /// <returns></returns>
    public IList<ExpressPhotoInfo> GetGoodsCreateExpressImageList(IList<string> trackingNoList) {
        IList<ExpressPhotoInfo> orderPhotoList = session.Query<ExpressPhotoInfo>()
            .Where(e => trackingNoList.Contains(e.TrackingNo))
            .ToList();

        foreach (ExpressPhotoInfo orderPhoto in orderPhotoList) {
            orderPhoto.SmallImageFullUrlCache = GetSmallImageFullUrl(orderPhoto.StoreLoc, orderPhoto.StorePath.Replace("\\", "/"), orderPhoto.StoreName);
            orderPhoto.BigImageFullUrlCache = GetBigImageFullUrl(orderPhoto.StoreLoc, orderPhoto.StorePath.Replace("\\", "/"), orderPhoto.StoreName, orderPhoto.CreateTime);
        }

        return orderPhotoList;
    }

    /// <summary>
    /// 这个方法是取快递单号图片里面的图片，而不是后来上传的
    /// 原来是这个名称 GetGoodsImageList
    /// </summary>
    /// <param name="goodsId"></param>
    /// <returns></returns>
    public IList<ExpressPhotoInfo> GetGoodsRecentOrderImageList(int goodsId) {
        ExpressPhotoInfo? ep = null;
        ExpressGoodsInfo? eg = null;

        IList<int> orderIdList = session.QueryOver<ExpressPhotoInfo>(() => ep!)
            .JoinEntityQueryOver(() => eg, () => ep!.Express!.Id == eg!.Express.Id) // 暂时用这个办法
            .Select(Projections.Distinct(Projections.Property($"{nameof(ep)}.{nameof(ep.Order)}.{nameof(ep.Order.Id)}")))
            .Where(() => eg!.Goods.Id == goodsId && ep!.Order != null)
            .OrderByAlias(() => ep!.Order!.Id).Desc
            .Take(3)
            .List<int>();

        IList<ExpressPhotoInfo> expressPhotoList = session.QueryOver<ExpressPhotoInfo>(() => ep!)
            .JoinEntityQueryOver(() => eg, () => ep!.Express!.Id == eg!.Express.Id) // 暂时用这个办法
            .Select(e => ep)
            // .Where(() => eg!.Goods.Id == goodsId && orderIdList.Contains(ep!.Order!.Id)) 这个会报错 不支持 Contains()
            .Where(Restrictions.And(Restrictions.Where(() => eg!.Goods.Id == goodsId), Restrictions.InG($"{nameof(ep)}.{nameof(ep.Order)}.{nameof(ep.Order.Id)}", orderIdList)))
            .List<ExpressPhotoInfo>();

        foreach (var expressPhoto in expressPhotoList) {
            expressPhoto.SmallImageFullUrlCache = GetSmallImageFullUrl(expressPhoto.StoreLoc, expressPhoto.StorePath, expressPhoto.StoreName);
            expressPhoto.BigImageFullUrlCache = GetBigImageFullUrl(expressPhoto.StoreLoc, expressPhoto.StorePath, expressPhoto.StoreName, expressPhoto.CreateTime);
        }

        return expressPhotoList;
    }

    private string GetBigImageFullUrl(ExpressPhotoStoreLoc storeLoc, string storePath, string storeName, DateTime createTime) {
        return GetBigImageFullUrl(storeLoc, storePath, storeName, createTime, OssSiteLocType.China);
    }

    private string GetBigImageFullUrl(ExpressPhotoStoreLoc storeLoc, string storePath, string storeName, DateTime createTime, OssSiteLocType siteLocType) {
        if (string.IsNullOrWhiteSpace(storeName) == false) {
            storeName = storeName.Replace('\\', '/');
        }

        if (storeLoc == ExpressPhotoStoreLoc.Oss) {
            string watermarkSafeBase64 = GetUrlSafeBase64($"【舜达集运】{createTime.ToString("yyyy-MM-dd HH:mm")} 拍摄");

            if (siteLocType == OssSiteLocType.HongKong) {
                return $"{AliYunOssConfig.UrlPrefixHongKong}/{storePath}/{storeName}?x-oss-process=image/watermark,type_d3F5LXplbmhlaQ,size_20,text_{watermarkSafeBase64},color_FF0000,g_south,y_80";
            } else {
                return $"{AliYunOssConfig.UrlPrefixChina}/{storePath}/{storeName}?x-oss-process=image/watermark,type_d3F5LXplbmhlaQ,size_20,text_{watermarkSafeBase64},color_FF0000,g_south,y_80";
            }
        } else {
            return $"{ExpressPhotoFileService.ImageUploadUrl}/{storePath}/{storeName}";
        }
    }

    private string GetSmallImageFullUrl(ExpressPhotoStoreLoc storeLoc, string storePath, string storeName) {
        return GetSmallImageFullUrl(storeLoc, storePath, storeName, OssSiteLocType.China);
    }

    private string GetSmallImageFullUrl(ExpressPhotoStoreLoc storeLoc, string storePath, string storeName, OssSiteLocType siteLocType) {
        if (string.IsNullOrWhiteSpace(storeName) == false) {
            storeName = storeName.Replace('\\', '/');
        }

        if (storeLoc == ExpressPhotoStoreLoc.Oss) {
            if (siteLocType == OssSiteLocType.HongKong) {
                return $"{AliYunOssConfig.UrlPrefixHongKong}/{storePath}/{storeName}?x-oss-process=image/resize,h_30,m_lfit";
            } else {
                return $"{AliYunOssConfig.UrlPrefixChina}/{storePath}/{storeName}?x-oss-process=image/resize,h_30,m_lfit";
            }
        } else {
            return $"{ExpressPhotoFileService.ImageUploadUrl}/{storePath}/{storeName}";
        }
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