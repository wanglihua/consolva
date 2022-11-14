using Microsoft.AspNetCore.Mvc;
using shunda.Core;
using shunda.Core.Web;
using shunda.Domain.Express;
using shunda.Domain.Pass;
using shunda.Service.Express;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Api.Controllers;

[NoLoginRequired]
[ApiArea]
public class AutoVideoUploadController : Controller
{
    private ISession session = null;

    private ExpressTrackingNoService expressTrackingNoService = null;
    private ExpressLogService expressLogService = null;

    public AutoVideoUploadController(ISession session,
        ExpressTrackingNoService expressTrackingNoService,
        ExpressLogService expressLogService)
    {
        this.session = session;
        this.expressTrackingNoService = expressTrackingNoService;
        this.expressLogService = expressLogService;
    }

    [HttpPost]
    [Route("/admin/api/auto/video/upload/camera")]
    public IActionResult uploadCamera(string storePath, string storeFileName, string expressTrackingNo, string uploaderLoginName)
    {
        // 现在 C++ 客户端也还没加上这个校验的
        // todo 参数使用 md5 校验，在这里做检查

        if (string.IsNullOrWhiteSpace(storePath))
        {
            throw new ValidationException("视频存储路径不能为空！");
        }

        if (string.IsNullOrWhiteSpace(storeFileName))
        {
            throw new ValidationException("视频文件名不能为空！");
        }

        if (string.IsNullOrWhiteSpace(expressTrackingNo))
        {
            throw new ValidationException("快递单号不能为空！");
        }

        if (string.IsNullOrWhiteSpace(expressTrackingNo) == false && expressTrackingNo.Length > 30)
        {
            throw new ValidationException("快递单号长度太长！");
        }

        if (string.IsNullOrEmpty(expressTrackingNo) == false && expressTrackingNoService.IsValidateTrackingNo(expressTrackingNo) == false)
        {
            throw new ValidationException("快递单号格式不符合规范！");
        }

        expressTrackingNo = expressTrackingNo.Trim().ToUpper();

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

        ExpressVideoType orderVideoType = ExpressVideoType.CheckIn;

        ExpressVideoInfo orderVideo = new ExpressVideoInfo();
        orderVideo.Customer = null;
        orderVideo.Order = null; // 3.0 入库时肯定还没订单
        orderVideo.Express = null;
        orderVideo.ExpressTrackingNo = expressTrackingNo;

        orderVideo.Type = orderVideoType;
        orderVideo.StorePath = storePath;
        orderVideo.StoreName = storeFileName;
        orderVideo.Status = ExpressVideoStatus.None;
        orderVideo.ExpressComName = String.Empty;
        orderVideo.StoreOss = true;
        orderVideo.User = uploadUser;

        session.Save(orderVideo);
        session.Flush();

        expressLogService.UserLog(orderVideo.ExpressTrackingNo, ExpressLogType.VideoUpload, $"类型:{orderVideoType.Name}", orderVideo.User?.Id ?? 0);

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "摄像头视频上传成功！" }
        });
    }
}