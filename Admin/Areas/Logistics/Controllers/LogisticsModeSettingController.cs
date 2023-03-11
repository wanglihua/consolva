using Microsoft.AspNetCore.Mvc;
using NLog;
using shunda.Admin.Areas.Logistics.Models.LogisticsModeSetting;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Logistics;
using shunda.Domain.Pass.PassRes;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Logistics.Controllers;

/// <summary>
/// 物流配置 控制器
/// </summary>
[LogisticsArea]
[AdminAuth(PassRes.Logistics.LogisticsModeSetting)]
public class LogisticsModeSettingController : Controller {
    private static Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;

    public LogisticsModeSettingController(ISession session) {
        this.session = session;
    }

    [HttpGet]
    [Route("/admin/logistics/logistics/mode/setting")]
    public IActionResult Index() {
        LogisticsModeSettingIndexModel model = new LogisticsModeSettingIndexModel();

        var setting = session.Query<LogisticsModeSettingInfo>()
            .OrderBy(e => e.Id)
            .Take(1)
            .SingleOrDefault();

        if (setting == null) {
            setting = new LogisticsModeSettingInfo();
        }

        model.LogisticsModeSettingId = setting.Id;

        model.VolumeWeightDenominator = setting.VolumeWeightDenominator;
        model.HasFreePackageLimit = setting.HasFreePackageLimit;
        model.FreePackageLimit = setting.FreePackageLimit;
        model.ExtraPackageFee = setting.ExtraPackageFee;
        model.HasExtraPackageFeeCapAmount = setting.HasExtraPackageFeeCapAmount;
        model.ExtraPackageFeeCapAmount = setting.ExtraPackageFeeCapAmount;

        return View("LogisticsModeSettingIndex", model);
    }

    [HttpPost]
    [Route("/admin/logistics/logistics/mode/setting/save")]
    [Transactional]
    public IActionResult Save(LogisticsModeSettingSaveModel model) {
        ValidationUtil.Validate<LogisticsModeSettingSaveModelValidator, LogisticsModeSettingSaveModel>(model);

        int logisticsModeSettingId = model.LogisticsModeSettingId;

        bool isCreate = logisticsModeSettingId <= 0;

        LogisticsModeSettingInfo logisticsModeSetting;

        if (isCreate) {
            logisticsModeSetting = new LogisticsModeSettingInfo();
        } else {
            logisticsModeSetting = session.Get<LogisticsModeSettingInfo>(logisticsModeSettingId);
            if (logisticsModeSetting == null) {
                throw new ValidationException($"指定的 物流配置#{logisticsModeSettingId} 不存在！");
            }
        }

        logisticsModeSetting.VolumeWeightDenominator = model.VolumeWeightDenominator;
        logisticsModeSetting.HasFreePackageLimit = model.HasFreePackageLimit;
        logisticsModeSetting.FreePackageLimit = model.FreePackageLimit;
        logisticsModeSetting.ExtraPackageFee = model.ExtraPackageFee;
        logisticsModeSetting.HasExtraPackageFeeCapAmount = model.HasExtraPackageFeeCapAmount;
        logisticsModeSetting.ExtraPackageFeeCapAmount = model.ExtraPackageFeeCapAmount;

        if (isCreate) {
            session.Save(logisticsModeSetting);
        } else {
            session.Update(logisticsModeSetting);
        }

        session.Flush();

        return Json(new JsonResultData {
            Success = true,
            Message = "保存成功！",
        });
    }
}