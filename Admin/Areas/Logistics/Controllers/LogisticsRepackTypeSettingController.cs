using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NLog;
using shunda.Admin.Areas.Logistics.Models.LogisticsRepackTypeSetting;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Logistics;
using shunda.Domain.Pass.PassRes;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Logistics.Controllers;

/// <summary>
/// 打包类型设置 控制器
/// </summary>
[LogisticsArea]
[AdminAuth(PassRes.Logistics.LogisticsRepackTypeSetting)]
public class LogisticsRepackTypeSettingController : Controller {
    private static Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;

    public LogisticsRepackTypeSettingController(ISession session) {
        this.session = session;
    }

    [HttpGet]
    [Route("/admin/logistics/logistics/repack/type/setting")]
    [Transactional]
    public IActionResult Index() {
        LogisticsRepackTypeSettingIndexModel model = new LogisticsRepackTypeSettingIndexModel();

        var repackTypeList = LogisticsRepackType.GetList();

        model.TypeList = repackTypeList;
        model.StatusList = LogisticsRepackTypeStatus.GetList();

        var settingList = session.Query<LogisticsRepackTypeSettingInfo>().ToList();

        foreach (var repackType in repackTypeList) {
            var setting = settingList.Where(e => e.Type == repackType).Take(1).SingleOrDefault();
            if (setting == null) {
                setting = new LogisticsRepackTypeSettingInfo {
                    Type = repackType,
                    Title = repackType.Name,
                    Description = repackType.Description,
                    OrderNum = repackType.Id,
                    Status = LogisticsRepackTypeStatus.Enabled,
                };

                session.Save(setting);
            }
        }

        session.Flush();

        return View("LogisticsRepackTypeSettingIndex", model);
    }

    [HttpPost]
    [Route("/admin/logistics/logistics/repack/type/setting/list")]
    public IActionResult List(LogisticsRepackTypeSettingListQueryParam queryParam) {
        var listQuery = GetQueryOver(queryParam);
        listQuery = listQuery.OrderBy(e => e.OrderNum).Asc;
        listQuery = listQuery.ThenByAlias(() => logisticsRepackTypeSettingAlias!.Id).Desc;

        IList<LogisticsRepackTypeSettingInfo> logisticsRepackTypeSettingList = listQuery
            .List<LogisticsRepackTypeSettingInfo>();

        /*
        foreach (LogisticsRepackTypeSettingInfo logisticsRepackTypeSetting in logisticsRepackTypeSettingList) {
        }
        */

        LogisticsRepackTypeSettingListModel model = new LogisticsRepackTypeSettingListModel {
            LogisticsRepackTypeSettingList = logisticsRepackTypeSettingList,
            LogisticsRepackTypeSettingAlias = nameof(logisticsRepackTypeSettingAlias),
        };

        return View("LogisticsRepackTypeSettingList", model);
    }

    private LogisticsRepackTypeSettingInfo? logisticsRepackTypeSettingAlias = null;

    [NonAction]
    private IQueryOver<LogisticsRepackTypeSettingInfo, LogisticsRepackTypeSettingInfo> GetQueryOver(LogisticsRepackTypeSettingListQueryParam queryParam) {
        var query = session.QueryOver<LogisticsRepackTypeSettingInfo>(() => logisticsRepackTypeSettingAlias!);

        return query;
    }

    [HttpPost]
    [Route("/admin/logistics/logistics/repack/type/setting/detail")]
    public IActionResult Detail(IdParam param) {
        var id = param.Id;

        var logisticsRepackTypeSetting = session.Get<LogisticsRepackTypeSettingInfo>(id);
        if (logisticsRepackTypeSetting == null) {
            throw new ValidationException($"指定的 打包类型设置#{id} 不存在！");
        }

        LogisticsRepackTypeSettingDetailModel model = new LogisticsRepackTypeSettingDetailModel();
        model.LogisticsRepackTypeSettingId = logisticsRepackTypeSetting.Id;

        model.Type = logisticsRepackTypeSetting.Type;
        model.Title = logisticsRepackTypeSetting.Title;
        model.Description = logisticsRepackTypeSetting.Description;
        model.OrderNum = logisticsRepackTypeSetting.OrderNum;
        model.Status = logisticsRepackTypeSetting.Status;

        model.TypeList = LogisticsRepackType.GetList();
        model.StatusList = LogisticsRepackTypeStatus.GetList();

        return View("LogisticsRepackTypeSettingDetail", model);
    }

    [HttpPost]
    [Route("/admin/logistics/logistics/repack/type/setting/save")]
    [Transactional]
    public IActionResult Save(LogisticsRepackTypeSettingSaveModel model) {
        ValidationUtil.Validate<LogisticsRepackTypeSettingSaveModelValidator, LogisticsRepackTypeSettingSaveModel>(model);

        int settingId = model.LogisticsRepackTypeSettingId;

        var setting = session.Get<LogisticsRepackTypeSettingInfo>(settingId);
        if (setting == null) {
            throw new ValidationException($"指定的 打包类型设置#{settingId} 不存在！");
        }

        // setting.Type = model.Type;
        setting.Title = model.Title;
        setting.Description = model.Description;
        setting.OrderNum = model.OrderNum;
        setting.Status = model.Status;

        session.Update(setting);
        session.Flush();

        return Json(new JsonResultData {
            Success = true,
            Message = "保存成功！",
        });
    }
}