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

        string storePath = Path.GetDirectoryName(photoUrl) ?? string.Empty;
        string storeName = Path.GetFileName(photoUrl);


        string trackingNo = fileName;
        if (string.IsNullOrWhiteSpace(trackingNo))
        {
            throw new ValidationException("快递单号不能都为空！");
        }

        if (string.IsNullOrWhiteSpace(trackingNo) == false)
        {
            trackingNo = trackingNo.Trim('\"');
        }

        // 子母单号，子单号图片放在母单号下面
        int indexOfMinus = trackingNo.IndexOf("-");
        if (indexOfMinus >= ExpressTrackingNoConfig.MinTrackingNoLength)
        {
            trackingNo = trackingNo.Substring(0, indexOfMinus);
        }

        if (string.IsNullOrWhiteSpace(trackingNo) == false && trackingNo.Length > 30)
        {
            throw new ValidationException("快递单号长度太长！");
        }

        if (expressTrackingNoService.IsValidateTrackingNo(trackingNo) == false)
        {
            throw new ValidationException("快递单号格式不符合规范！");
        }

        trackingNo = trackingNo.Trim().ToUpper();

        // 虚拟上传人
        UserInfo? user = null;
        if (string.IsNullOrWhiteSpace(uploaderLoginName) == false)
        {
            uploaderLoginName = uploaderLoginName.Trim();
            user = session.Query<UserInfo>()
                .Where(e => e.LoginName == uploaderLoginName)
                .Take(1)
                .SingleOrDefault();
        }

        ExpressInfo? express = session.Query<ExpressInfo>()
            .Where(e => e.TrackingNo == trackingNo)
            .Take(1)
            .SingleOrDefault();

        CustomerInfo? customer = express?.Customer; // 可能为空
        LogisticsOrderInfo? order = express?.Order; // 可能为空
        string orderCode = order?.Code ?? string.Empty;

        ExpressPhotoInfo expressPhoto = new ExpressPhotoInfo();

        expressPhoto.Status = ExpressPhotoStatus.Normal;
        expressPhoto.User = user;

        expressPhoto.TrackingNo = trackingNo;
        expressPhoto.ExpressCompany = string.Empty;
        expressPhoto.Express = express;
        expressPhoto.Customer = customer;
        expressPhoto.Order = order;
        expressPhoto.OrderCode = orderCode;


        expressPhoto.Type = expressPhotoType;
        expressPhoto.StoreLoc = ExpressPhotoStoreLoc.Oss;
        expressPhoto.StorePath = storePath;
        expressPhoto.StoreName = storeName;

        session.Save(expressPhoto);
        session.Flush();

        if (expressPhoto.User == null)
        {
            expressLogService.Log(expressPhoto.TrackingNo, ExpressLogType.PhotoUpload, $"类型:{expressPhotoType.Name}");
        }
        else
        {
            expressLogService.UserLog(expressPhoto.TrackingNo, ExpressLogType.PhotoUpload, $"类型:{expressPhotoType.Name}", expressPhoto.User?.Id ?? 0);
        }

        // todo a  发送快递单号图片信息
        // if (expressPhoto.Order != null)
        // {
        //     try
        //     {
        //         expressPhotoService.SendAddOrderPhotoMessage(expressPhoto);
        //     }
        //     catch (Exception exp)
        //     {
        //         log.Error(exp);
        //     }
        // }

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
        string orderCode = order?.Code ?? string.Empty;

        ExpressPhotoType orderPhotoType = ExpressPhotoType.CheckIn;

        ExpressPhotoInfo orderPhoto = new ExpressPhotoInfo();
        orderPhoto.TrackingNo = trackingNo;

        orderPhoto.Type = orderPhotoType;
        orderPhoto.StorePath = storePath;
        orderPhoto.StoreName = storeFileName;
        orderPhoto.Status = ExpressPhotoStatus.None;
        orderPhoto.ExpressCompany = String.Empty;
        orderPhoto.StoreLoc = ExpressPhotoStoreLoc.Oss;
        orderPhoto.User = uploadUser;

        orderPhoto.Express = express;
        orderPhoto.Customer = customer;
        orderPhoto.Order = order;
        orderPhoto.OrderCode = orderCode;

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
            ep.OrderCode = orderCode;

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
            ev.OrderCode = orderCode;

            session.Update(ev);
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "摄像头图片上传成功！" }
        });
    }
}