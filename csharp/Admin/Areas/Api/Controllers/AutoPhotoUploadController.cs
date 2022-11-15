using Microsoft.AspNetCore.Mvc;
using NLog;
using shunda.Core;
using shunda.Core.Config;
using shunda.Core.Web;
using shunda.Domain.Customer;
using shunda.Domain.Express;
using shunda.Domain.Order;
using shunda.Domain.Pass;
using shunda.Service.Express;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Api.Controllers;

[NoLoginRequired]
[ApiArea]
public class AutoPhotoUploadController : Controller
{
    private static Logger log = LogManager.GetCurrentClassLogger();

    private ISession session;
    private ExpressTrackingNoService expressTrackingNoService;
    private ExpressPhotoService expressPhotoService;
    private ExpressLogService expressLogService;

    public AutoPhotoUploadController(ISession session,
        ExpressTrackingNoService expressTrackingNoService,
        ExpressPhotoService expressPhotoService,
        ExpressLogService expressLogService)
    {
        this.session = session;
        this.expressTrackingNoService = expressTrackingNoService;
        this.expressPhotoService = expressPhotoService;
        this.expressLogService = expressLogService;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="photoUrl"></param>
    /// <param name="uploaderLoginName"></param>
    /// <param name="type">可以是 sign，可以是 load，默认是 sign</param>
    /// <returns></returns>
    /// <exception cref="ValidationException"></exception>
    [HttpPost]
    [Route("/admin/api/auto/photo/upload/create")]
    public IActionResult Create(string fileName, string photoUrl, string uploaderLoginName, string type)
    {
        // 现在golang客户端也还没加上这个校验的
        // todo 参数使用 md5 校验，在这里做检查

        if (string.IsNullOrWhiteSpace(photoUrl))
        {
            throw new ValidationException("图片URL不能为空！");
        }

        if (string.IsNullOrWhiteSpace(uploaderLoginName))
        {
            throw new ValidationException("上传人登录名不能为空！");
        }

        ExpressPhotoType expressPhotoType = ExpressPhotoType.SignIn;
        if (string.IsNullOrWhiteSpace(type) == false)
        {
            if (type == "sign")
            {
                expressPhotoType = ExpressPhotoType.SignIn;
            }

            // if (type == "load") 当前只有sign一种情况
            // {
            //     expressPhotoType = ExpressPhotoType.Load;
            // }
        }

        string storePath = Path.GetDirectoryName(photoUrl);
        string storeName = Path.GetFileName(photoUrl);

        ExpressPhotoInfo orderPhoto = new ExpressPhotoInfo();

        if (expressPhotoType == ExpressPhotoType.SignIn)
        {
            string expressTrackingNo = fileName;
            if (string.IsNullOrWhiteSpace(expressTrackingNo))
            {
                throw new ValidationException("快递单号不能都为空！");
            }

            if (string.IsNullOrWhiteSpace(expressTrackingNo) == false)
            {
                expressTrackingNo = expressTrackingNo.Trim('\"');
            }

            // 子母单号，子单号图片放在母单号下面
            int indexOfMinus = expressTrackingNo.IndexOf("-");
            if (indexOfMinus >= ExpressTrackingNoConfig.MinTrackingNoLength)
            {
                expressTrackingNo = expressTrackingNo.Substring(0, indexOfMinus);
            }

            if (string.IsNullOrWhiteSpace(expressTrackingNo) == false && expressTrackingNo.Length > 30)
            {
                throw new ValidationException("快递单号长度太长！");
            }

            if (string.IsNullOrEmpty(expressTrackingNo) == false && expressTrackingNoService.IsValidateTrackingNo(expressTrackingNo) == false)
            {
                throw new ValidationException("快递单号格式不符合规范！");
            }

            if (string.IsNullOrWhiteSpace(expressTrackingNo) == false)
            {
                expressTrackingNo = expressTrackingNo.Trim().ToUpper();
            }


            LogisticsOrderGoodsExpressInfo orderGoodsExpress = null;
            if (string.IsNullOrWhiteSpace(expressTrackingNo) == false)
            {
                orderGoodsExpress = session.Query<LogisticsOrderGoodsExpressInfo>()
                    .Fetch(e => e.Order).ThenFetch(e => e.Customer)
                    .Where(e => e.ExpressTrackingNo == expressTrackingNo
                                && e.Order.OrderStatus != LogisticsOrderStatus.Draft
                                && e.Order.OrderStatus != LogisticsOrderStatus.OrderNormalCompleted
                                && e.Order.OrderStatus != LogisticsOrderStatus.OrderAbnormalCompleted)
                    .Take(1)
                    .SingleOrDefault();
            }

            orderPhoto.Customer = orderGoodsExpress?.Order?.Customer;
            orderPhoto.Order = orderGoodsExpress?.Order;
            orderPhoto.Express = orderGoodsExpress;
            orderPhoto.ExpressTrackingNo = expressTrackingNo;
        }


        // 这里只有 sign in，还没有 load 的情况 
        // if (expressPhotoType == ExpressPhotoType.Load)
        // {
        //     string orderPackBoxCode = fileName;
        //     LogisticsOrderPackBoxInfo orderPackBox = logisticsOrderPackBoxService.ParseBoxCode(orderPackBoxCode); // 必须可以是小标签
        //     if (orderPackBox == null)
        //     {
        //         throw new ValidationException("指定的箱号不存在！");
        //     }
        //
        //     orderPhoto.Customer = orderPackBox.Customer;
        //     orderPhoto.Order = orderPackBox.OrderPack?.Order;
        // }

        orderPhoto.Type = expressPhotoType;
        orderPhoto.StorePath = storePath;
        orderPhoto.StoreName = storeName;
        orderPhoto.Status = ExpressPhotoStatus.None;
        orderPhoto.ExpressComName = String.Empty;
        orderPhoto.StoreOss = true;

        // 虚拟上传人
        if (string.IsNullOrWhiteSpace(uploaderLoginName))
        {
            orderPhoto.User = null;
        }
        else
        {
            uploaderLoginName = uploaderLoginName.Trim();

            UserInfo? uploadUser = session.Query<UserInfo>()
                .Where(e => e.LoginName == uploaderLoginName)
                .Take(1)
                .SingleOrDefault();

            if (uploadUser == null)
            {
                orderPhoto.User = null;
            }
            else
            {
                orderPhoto.User = uploadUser;
            }
        }

        session.Save(orderPhoto);
        session.Flush();

        if (orderPhoto.User == null)
        {
            expressLogService.Log(orderPhoto.TrackingNo, ExpressLogType.PhotoUpload, $"类型:{expressPhotoType.Name}");
        }
        else
        {
            expressLogService.UserLog(orderPhoto.TrackingNo, ExpressLogType.PhotoUpload, $"类型:{expressPhotoType.Name}", orderPhoto.User?.Id ?? 0);
        }

        if (orderPhoto.Order != null)
        {
            try
            {
                expressPhotoService.SendAddOrderPhotoMessage(orderPhoto);
            }
            catch (Exception exp)
            {
                log.Error(exp);
            }
        }

        // 同快递单号图片视频绑定

        IList<ExpressPhotoInfo> expressPhotoList = session.Query<ExpressPhotoInfo>()
            .Where(e => e.TrackingNo == trackingNo)
            .ToList();
        foreach (ExpressPhotoInfo ep in expressPhotoList)
        {
            ep.Express = express;
            ep.Customer = customer;
            ep.Order = order;
            ep.OrderCode = order?.Code ?? string.Empty;

            session.Update(ep);
        }

        session.Flush();

        IList<ExpressVideoInfo> expressVideoList = session.Query<ExpressVideoInfo>()
            .Where(e => e.TrackingNo == trackingNo)
            .ToList();
        foreach (ExpressVideoInfo ev in expressVideoList)
        {
            ev.Express = express;
            ev.Customer = customer;
            ev.Order = order;
            ev.OrderCode = order?.Code ?? string.Empty;

            session.Update(ev);
        }

        session.Flush();

        return Content("success"); // 方便客户端 golang 处理
    }

    [HttpPost]
    [Route("/admin/api/auto/photo/upload/camera")]
    public IActionResult uploadCamera(string storePath, string storeFileName, string trackingNo, string uploaderLoginName)
    {
        // 现在 C++ 客户端也还没加上这个校验的
        // todo 参数使用 md5 校验，在这里做检查

        if (string.IsNullOrWhiteSpace(storePath))
        {
            throw new ValidationException("图片存储路径不能为空！");
        }

        if (string.IsNullOrWhiteSpace(storeFileName))
        {
            throw new ValidationException("图片文件名不能为空！");
        }

        if (string.IsNullOrWhiteSpace(trackingNo))
        {
            throw new ValidationException("快递单号不能为空！");
        }

        if (string.IsNullOrWhiteSpace(trackingNo) == false && trackingNo.Length > 30)
        {
            throw new ValidationException("快递单号长度太长！");
        }

        if (string.IsNullOrEmpty(trackingNo) == false && expressTrackingNoService.IsValidateTrackingNo(trackingNo) == false)
        {
            throw new ValidationException("快递单号格式不符合规范！");
        }

        trackingNo = trackingNo.Trim().ToUpper();

        if (string.IsNullOrWhiteSpace(uploaderLoginName))
        {
            throw new ValidationException("入库操作人登录名不能为空！");
        }

        // 入库操作人
        uploaderLoginName = uploaderLoginName.Trim();

        UserInfo? uploadUser = session.Query<UserInfo>()
            .Where(e => e.LoginName == uploaderLoginName)
            .Take(1)
            .SingleOrDefault();
        if (uploadUser == null)
        {
            throw new ValidationException($"指定的入库操作人登录名#{uploaderLoginName}不存在！");
        }

        ExpressInfo? express = session.Query<ExpressInfo>()
            .Where(e => e.TrackingNo == trackingNo)
            .Take(1)
            .SingleOrDefault();

        CustomerInfo? customer = express?.Customer; // 可能为空
        LogisticsOrderInfo? order = express?.Order; // 可能为空

        ExpressPhotoType orderPhotoType = ExpressPhotoType.CheckIn;

        ExpressPhotoInfo orderPhoto = new ExpressPhotoInfo();
        orderPhoto.TrackingNo = trackingNo;

        orderPhoto.Type = orderPhotoType;
        orderPhoto.StorePath = storePath;
        orderPhoto.StoreName = storeFileName;
        orderPhoto.Status = ExpressPhotoStatus.None;
        orderPhoto.ExpressComName = String.Empty;
        orderPhoto.StoreLoc = ExpressPhotoStoreLoc.Oss;
        orderPhoto.User = uploadUser;

        orderPhoto.Express = express;
        orderPhoto.Customer = customer;
        orderPhoto.Order = order;
        orderPhoto.OrderCode = order?.Code ?? string.Empty;

        session.Save(orderPhoto);
        session.Flush();

        expressLogService.UserLog(orderPhoto.TrackingNo, ExpressLogType.PhotoUpload, $"类型:{orderPhotoType.Name}", orderPhoto.User?.Id ?? 0);

        // 同快递单号图片视频绑定
        IList<ExpressPhotoInfo> expressPhotoList = session.Query<ExpressPhotoInfo>()
            .Where(e => e.TrackingNo == trackingNo)
            .ToList();
        foreach (ExpressPhotoInfo ep in expressPhotoList)
        {
            ep.Express = express;
            ep.Customer = customer;
            ep.Order = order;
            ep.OrderCode = order?.Code ?? string.Empty;

            session.Update(ep);
        }

        session.Flush();

        IList<ExpressVideoInfo> expressVideoList = session.Query<ExpressVideoInfo>()
            .Where(e => e.TrackingNo == trackingNo)
            .ToList();
        foreach (ExpressVideoInfo ev in expressVideoList)
        {
            ev.Express = express;
            ev.Customer = customer;
            ev.Order = order;
            ev.OrderCode = order?.Code ?? string.Empty;

            session.Update(ev);
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "摄像头图片上传成功！" }
        });
    }
}