using Microsoft.AspNetCore.Mvc;
using shunda.Admin.Areas.ReceivePack.Models.ExpressCheckinConfig;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Express;
using shunda.Domain.Pass.PassRes;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.ReceivePack.Controllers;

[ReceivePackArea]
[AdminAuth(PassRes.ReceivePack.ExpressCheckinConfig)]
public class ExpressCheckinConfigController : Controller {
    private readonly ISession session;

    public ExpressCheckinConfigController(ISession session) {
        this.session = session;
    }

    [HttpGet]
    [Route("/admin/express/checkin/config")]
    public IActionResult Index() {
        session.DefaultReadOnly = true;

        ExpressCheckinConfigInfo? expressCheckinConfig = session
            .Query<ExpressCheckinConfigInfo>()
            .OrderByDescending(e => e.Id)
            .Take(1)
            .SingleOrDefault();

        if (expressCheckinConfig == null) {
            expressCheckinConfig = new ExpressCheckinConfigInfo();
        }

        ExpressCheckinConfigIndexModel model = new ExpressCheckinConfigIndexModel();
        model.ExpressCheckinLabelStatusList = ExpressCheckinLabelConfigStatus.GetList();
        model.ExpressCheckinConfig = expressCheckinConfig;

        return View("ExpressCheckinConfigIndex", model);
    }

    [HttpPost]
    [Route("/admin/express/checkin/config/save")]
    [Transactional]
    public IActionResult Save(ExpressCheckinConfigIndexModel model) {
        if (!ModelState.IsValid) {
            throw new ValidationException(ValidationUtil.GetErrorMessages(ModelState));
        }

        ExpressCheckinConfigInfo expressCheckinConfigFromUi = model.ExpressCheckinConfig;

        ExpressCheckinConfigInfo? expressCheckinConfig = session
            .Query<ExpressCheckinConfigInfo>()
            .OrderByDescending(e => e.Id)
            .Take(1)
            .SingleOrDefault();

        bool isCreate = false;
        if (expressCheckinConfig == null) {
            isCreate = true;
            expressCheckinConfig = new ExpressCheckinConfigInfo();
        }

        expressCheckinConfig.ExpressCountPerStorageLoc = expressCheckinConfigFromUi.ExpressCountPerStorageLoc;
        expressCheckinConfig.SigninPhotoMinQtyPerExpress = expressCheckinConfigFromUi.SigninPhotoMinQtyPerExpress;
        expressCheckinConfig.CheckinPhotoMinQtyPerExpress = expressCheckinConfigFromUi.CheckinPhotoMinQtyPerExpress;
        expressCheckinConfig.CheckinPhotoClientShow = expressCheckinConfigFromUi.CheckinPhotoClientShow;
        expressCheckinConfig.AutoRecordCheckinVideo = expressCheckinConfigFromUi.AutoRecordCheckinVideo;
        expressCheckinConfig.ClientShowVideo = expressCheckinConfigFromUi.ClientShowVideo;

        expressCheckinConfig.LabelW10H05Status = expressCheckinConfigFromUi.LabelW10H05Status;
        expressCheckinConfig.LabelW10H05PrintCount = expressCheckinConfigFromUi.LabelW10H05PrintCount;

        expressCheckinConfig.LabelW05H05Status = expressCheckinConfigFromUi.LabelW05H05Status;
        expressCheckinConfig.LabelW05H05PrintCount = expressCheckinConfigFromUi.LabelW05H05PrintCount;

        if (isCreate) {
            session.Save(expressCheckinConfig);
        } else {
            session.Update(expressCheckinConfig);
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "保存成功！" },
        });
    }

    [HttpGet]
    [Route("/admin/express/checkin/config/print/preview/{labelTypeId:int}")]
    public IActionResult PrintPreview(int labelTypeId) {
        ExpressCheckinConfigPrintPreviewModel model = new ExpressCheckinConfigPrintPreviewModel();
        model.LabelType = ExpressCheckinLabelType.GetById(labelTypeId);

        return View("ExpressCheckinConfigPrintPreview", model);
    }

    [HttpPost]
    [Route("/admin/express/checkin/config/get/express")]
    public IActionResult GetExpress(string expressTrackingNo) {
        if (string.IsNullOrWhiteSpace(expressTrackingNo)) {
            throw new ValidationException("快递单号不能为空！");
        }

        expressTrackingNo = expressTrackingNo.Trim();

        ExpressInfo? express = session.Query<ExpressInfo>()
            .Where(e => e.TrackingNo == expressTrackingNo)
            .Take(1)
            .SingleOrDefault();

        if (express == null) {
            throw new ValidationException("指定的快递单号不存在！");
        }

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "获取数据成功！" },
            { "id", express.Id }
        });
    }
}