using Microsoft.AspNetCore.Mvc;
using NLog;
using shunda.Admin.Areas.App.Models.ExpressPhoto;
using shunda.Admin.Services;
using shunda.Core;
using shunda.Core.Config;
using shunda.Core.Hibernate;
using shunda.Core.Web;
using shunda.Domain.Customer;
using shunda.Domain.Express;
using shunda.Domain.LogisticsOrder;
using shunda.Domain.Pass;
using shunda.Domain.Pass.PassRes;
using shunda.Service.Express;
using shunda.Service.LogisticsOrder;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.App.Controllers;

[AppArea]
[AdminAuth(
    PassRes.AdminApp.PhotoSignIn,
    PassRes.AdminApp.PhotoCheckIn,
    PassRes.AdminApp.PhotoIssue,
    PassRes.AdminApp.PhotoPack,
    PassRes.AdminApp.PhotoReturn,
    PassRes.AdminApp.PhotoShip
)]
public class ExpressPhotoController : Controller {
    private static Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;
    private readonly LoginUserService loginUserService;
    private readonly ExpressTrackingNoService expressTrackingNoService;
    private readonly LogisticsOrderPackBoxService logisticsOrderPackBoxService;
    private readonly ExpressPhotoMessageService expressPhotoMessageService;
    private readonly ExpressLogService expressLogService;
    private readonly ExpressPhotoFileService expressPhotoFileService;

    public ExpressPhotoController(
        ISession session,
        LoginUserService loginUserService,
        ExpressTrackingNoService expressTrackingNoService,
        LogisticsOrderPackBoxService logisticsOrderPackBoxService,
        ExpressPhotoMessageService expressPhotoMessageService,
        ExpressLogService expressLogService,
        ExpressPhotoFileService expressPhotoFileService) {
        this.session = session;
        this.loginUserService = loginUserService;
        this.expressTrackingNoService = expressTrackingNoService;
        this.logisticsOrderPackBoxService = logisticsOrderPackBoxService;
        this.expressPhotoMessageService = expressPhotoMessageService;
        this.expressLogService = expressLogService;
        this.expressPhotoFileService = expressPhotoFileService;
    }

    [NonAction]
    private string CheckExpressTrackingNoRepeat(string expressTrackingNo, ExpressPhotoType photoType) {
        if (string.IsNullOrWhiteSpace(expressTrackingNo)) {
            return string.Empty;
        }

        expressTrackingNo = expressTrackingNo.Trim();

        DateTime twentyFourHoursAgo = DateTime.Now.AddHours(-24);
        var twentyFourHoursAgoPhoto = session.Query<ExpressPhotoInfo>()
            .Where(e => e.TrackingNo == expressTrackingNo
                        && e.Type == photoType
                        && e.CreateTime < twentyFourHoursAgo)
            .Take(1)
            .SingleOrDefault();

        if (twentyFourHoursAgoPhoto != null) {
            return "对24小时之外相同的快递单号进行同类型拍照，请检查是不是快递单号有误！";
        }

        return string.Empty;
    }

    /// <summary>
    /// 创建已经在阿里云OSS存储了的图片信息
    /// </summary>
    /// <param name="orderCode">订单编号，也可以是箱号吧</param>
    /// <param name="trackingNo"></param>
    /// <param name="photoTypeId"></param>
    /// <param name="imageFileName"></param>
    /// <param name="clientTime">2022-01-01 00:00:00格式字符串</param>
    /// <returns></returns>
    /// <exception cref="ValidationException"></exception>
    [HttpPost]
    [Route("/admin/app/express/photo/create")]
    [Transactional]
    public IActionResult Create(string orderCode, string trackingNo, int photoTypeId, string imageFileName, string clientTime) {
        int loginUserId = loginUserService.GetLoginUserId();
        var loginUser = loginUserService.GetLoginUser();

        if (string.IsNullOrWhiteSpace(trackingNo)) {
            trackingNo = string.Empty;
        }

        // 子母单号，子单号图片放在母单号下面
        if (string.IsNullOrWhiteSpace(trackingNo) == false) {
            int indexOfMinus = trackingNo.IndexOf("-");
            if (indexOfMinus >= ExpressTrackingNoConfig.MinTrackingNoLength) {
                trackingNo = trackingNo.Substring(0, indexOfMinus);
            }
        }

        if (string.IsNullOrWhiteSpace(trackingNo) == false && expressTrackingNoService.IsValidateTrackingNo(trackingNo) == false) {
            throw new ValidationException("快递单号格式不符合规范！");
        }

        //  不需要后缀
        // if (string.IsNullOrWhiteSpace(trackingNo) == false && trackingNo.EndsWith(CheckinLabelConfig.ExpressTrackingNoShundaSuffix)) {
        //     trackingNo = trackingNo.Substring(0, trackingNo.Length - CheckinLabelConfig.ExpressTrackingNoShundaSuffix.Length);
        // }

        if (string.IsNullOrWhiteSpace(trackingNo) == false) {
            trackingNo = expressTrackingNoService.RemoveUselessSuffix(trackingNo);
        }

        if (string.IsNullOrWhiteSpace(imageFileName)) {
            throw new ValidationException("图片文件名称不能为空！");
        }

        ExpressPhotoType photoType = ExpressPhotoType.GetById(photoTypeId);
        if (photoType == null || photoType == ExpressPhotoType.None) {
            throw new ValidationException("未指定订单图片类型！");
        }

        // 如果 orderId <= 0 表示没有指定 orderId

        if (string.IsNullOrWhiteSpace(orderCode) && string.IsNullOrWhiteSpace(trackingNo)) {
            throw new ValidationException("订单和快递单号不能都为空！");
        }

        if (string.IsNullOrWhiteSpace(orderCode) == false && orderCode.Length > 100) {
            throw new ValidationException("订单编号长度太长！");
        }

        if (string.IsNullOrWhiteSpace(trackingNo) == false && trackingNo.Length > 30) {
            throw new ValidationException("快递单号长度太长！");
        }

        if (string.IsNullOrWhiteSpace(trackingNo) == false) {
            trackingNo = trackingNo.Trim().ToUpper();
        }

        string checkResult = CheckExpressTrackingNoRepeat(trackingNo, photoType);

        LogisticsOrderInfo? order = null;
        if (string.IsNullOrWhiteSpace(orderCode) == false) {
            order = session.Query<LogisticsOrderInfo>()
                .Where(e => e.Code == orderCode)
                .Take(1)
                .SingleOrDefault();

            if (order == null) {
                var orderPackBox = logisticsOrderPackBoxService.ParseBoxCode(orderCode, throwException: false);
                if (orderPackBox != null) {
                    order = orderPackBox.Order;
                }
            }

            if (order == null) {
                throw new ValidationException($"指定的订单编号或箱号{orderCode}不存在！");
            }
        }

        ExpressInfo? express = null;
        if (string.IsNullOrWhiteSpace(trackingNo) == false) {
            express = session.Query<ExpressInfo>().Where(e => e.TrackingNo == trackingNo).Take(1).SingleOrDefault();
        }

        // 订单和快递单号都提供了，并且快递单号的录入记录存在，但不匹配
        if (order != null && string.IsNullOrWhiteSpace(trackingNo) == false && (express?.Order?.Id ?? 0) != order.Id) {
            throw new ValidationException("快递单号和指定的订单不匹配，可能快递单号属于别的订单！");
        }

        // 如果提供了快递单号，但没提供订单
        if (express != null && order == null) {
            order = express.Order;
        }

        CustomerInfo? customer = express?.Customer;
        if (customer == null) {
            customer = order?.Customer;
        }

        string storePath = Path.GetDirectoryName(imageFileName) ?? string.Empty;
        string storeName = Path.GetFileName(imageFileName);

        var expressPhoto = new ExpressPhotoInfo {
            Customer = customer,
            Order = order,
            OrderCode = order?.Code ?? string.Empty,
            Express = express,
            TrackingNo = trackingNo,
            Type = photoType,
            StorePath = storePath,
            StoreName = storeName,
            User = loginUser,
            Status = ExpressPhotoStatus.Normal,
            ExpressCompany = String.Empty,
            StoreLoc = ExpressPhotoStoreLoc.Oss
        };

        if (loginUserId != 0) {
            expressPhoto.User = session.Get<UserInfo>(loginUserId);
        } else {
            expressPhoto.User = null;
        }

        session.Save(expressPhoto);
        session.Flush();

        if (string.IsNullOrWhiteSpace(trackingNo) == false) {
            if (expressPhoto.User == null) {
                expressLogService.Log(expressPhoto.TrackingNo, ExpressLogType.PhotoUpload, $"类型:{expressPhoto.Type.Name}");
            } else {
                expressLogService.UserLog(expressPhoto.TrackingNo, ExpressLogType.PhotoUpload, $"类型:{expressPhoto.Type.Name}", expressPhoto.User?.Id ?? 0);
            }
        }

        // 只有已入库快递单的图片，才发送消息
        if (expressPhoto.Order != null || expressPhoto.Express != null) {
            // 调用的内部已经处理消息重复发送问题
            expressPhotoMessageService.SendAddPhotoMessage(expressPhoto);
        }

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "照片上传成功！" + checkResult },
        });
    }

    [HttpPost]
    [Route("/admin/app/express/photo/get/order/code/by/pack/box/code")]
    public IActionResult GetOrderCodeByPackBoxCode(string packBoxCode) {
        var packBox = logisticsOrderPackBoxService.ParseBoxCode(packBoxCode);

        if (packBox == null) {
            throw new ValidationException($"箱号{packBoxCode}不存在！");
        }

        int orderId = packBox.Order.Id;
        LogisticsOrderInfo order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null) {
            throw new ValidationException($"箱号{packBoxCode}对应的订单不存在！");
        }

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new ExpressPhotoGetOrderCodeByPackBoxCodeModel {
                PackBoxCode = logisticsOrderPackBoxService.GetBoxCode(packBox.Id),
                OrderCode = order.Code,
            }
        });
    }

    [HttpPost]
    [Route("/admin/app/express/photo/express/photo/list")]
    public IActionResult ExpressPhotoList(string trackingNo) {
        if (string.IsNullOrWhiteSpace(trackingNo) == true) {
            throw new ValidationException("快递单号不能为空！");
        }

        IList<ExpressPhotoInfo> photoList = session
            .Query<ExpressPhotoInfo>()
            .Where(e => e.TrackingNo == trackingNo)
            .OrderBy(e => e.Id)
            .ToList();

        IList<IDictionary<string, object>> imageList = new List<IDictionary<string, object>>();
        foreach (var photo in photoList) {
            IDictionary<string, object> item = new Dictionary<string, object>();
            item["id"] = photo.Id;

            string smallImageUrl = expressPhotoFileService.GetSmallImageFullUrlForApp(photo, OssSiteLocType.China);
            item["smallImageUrl"] = smallImageUrl;

            string bigImageUrl = expressPhotoFileService.GetBigImageFullUrlForApp(photo, OssSiteLocType.China);
            item["bigImageUrl"] = bigImageUrl;

            // log.Trace(bigImageUrl);

            imageList.Add(item);
        }

        return Json(new JsonResultData {
            Success = true,
            Message = $"快递单 {trackingNo} 图片获取成功！",
            Data = new Dictionary<string, object>() {
                { "trackingNo", trackingNo },
                { "photoList", imageList }
            }
        });
    }
}