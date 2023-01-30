using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using shunda.Core;
using shunda.Core.Web;
using shunda.Domain.Customer;
using shunda.Domain.Express;
using shunda.Domain.LogisticsOrder;
using shunda.Domain.Pass;
using shunda.Service.Express;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.AutoApi.Controllers;

[NoLoginRequired]
[AutoApiArea]
public class VideoUploadController : Controller {
    private readonly ISession session;

    private readonly ExpressTrackingNoService expressTrackingNoService;
    private readonly ExpressLogService expressLogService;

    public VideoUploadController(ISession session,
        ExpressTrackingNoService expressTrackingNoService,
        ExpressLogService expressLogService) {
        this.session = session;
        this.expressTrackingNoService = expressTrackingNoService;
        this.expressLogService = expressLogService;
    }

    [HttpPost]
    [Route("/admin/auto/api/video/upload/camera")]
    public IActionResult uploadCamera(string storePath, string storeFileName, string trackingNo, string uploaderLoginName) {
        // 现在 C++ 客户端也还没加上这个校验的
        // todo 参数使用 md5 校验，在这里做检查

        if (string.IsNullOrWhiteSpace(storePath)) {
            throw new ValidationException("视频存储路径不能为空！");
        }

        if (string.IsNullOrWhiteSpace(storeFileName)) {
            throw new ValidationException("视频文件名不能为空！");
        }

        if (string.IsNullOrWhiteSpace(trackingNo)) {
            throw new ValidationException("快递单号不能为空！");
        }

        if (string.IsNullOrWhiteSpace(trackingNo) == false && trackingNo.Length > 30) {
            throw new ValidationException("快递单号长度太长！");
        }

        if (string.IsNullOrEmpty(trackingNo) == false && expressTrackingNoService.IsValidateTrackingNo(trackingNo) == false) {
            throw new ValidationException("快递单号格式不符合规范！");
        }

        trackingNo = trackingNo.Trim().ToUpper();

        if (string.IsNullOrWhiteSpace(uploaderLoginName)) {
            throw new ValidationException("入库操作人登录名不能为空！");
        }

        // 入库操作人
        uploaderLoginName = uploaderLoginName.Trim();

        UserInfo? uploadUser = session.Query<UserInfo>()
            .Where(e => e.LoginName == uploaderLoginName)
            .Take(1)
            .SingleOrDefault();
        if (uploadUser == null) {
            throw new ValidationException($"指定的入库操作人登录名#{uploaderLoginName}不存在！");
        }

        ExpressInfo? express = session.Query<ExpressInfo>()
            .Where(e => e.TrackingNo == trackingNo)
            .Take(1)
            .SingleOrDefault();

        CustomerInfo? customer = express?.Customer; // 可能为空
        LogisticsOrderInfo? order = express?.Order; // 可能为空
        string orderCode = order?.Code ?? string.Empty;

        ExpressVideoType orderVideoType = ExpressVideoType.CheckIn;

        ExpressVideoInfo orderVideo = new ExpressVideoInfo();
        orderVideo.TrackingNo = trackingNo;

        orderVideo.Type = orderVideoType;
        orderVideo.StorePath = storePath;
        orderVideo.StoreName = storeFileName;
        orderVideo.Status = ExpressVideoStatus.None;
        orderVideo.ExpressCompany = String.Empty;
        orderVideo.StoreLoc = ExpressPhotoStoreLoc.Oss;
        orderVideo.User = uploadUser;

        orderVideo.Express = express;
        orderVideo.Customer = customer;
        orderVideo.Order = order;
        orderVideo.OrderCode = orderCode;

        session.Save(orderVideo);
        session.Flush();

        expressLogService.UserLog(orderVideo.TrackingNo, ExpressLogType.VideoUpload, $"类型:{orderVideoType.Name}", orderVideo.User?.Id ?? 0);

        // 同快递单号图片视频绑定
        IList<ExpressPhotoInfo> expressPhotoList = session.Query<ExpressPhotoInfo>()
            .Where(e => e.TrackingNo == trackingNo)
            .ToList();
        foreach (ExpressPhotoInfo ep in expressPhotoList) {
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
        foreach (ExpressVideoInfo ev in expressVideoList) {
            ev.Express = express;
            ev.Customer = customer;
            ev.Order = order;
            ev.OrderCode = orderCode;

            session.Update(ev);
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "摄像头视频上传成功！" }
        });
    }
}