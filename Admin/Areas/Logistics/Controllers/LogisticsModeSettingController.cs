using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Logistics;
using shunda.Admin.Areas.Logistics.Models.LogisticsModeSetting;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NLog;
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


        return View("LogisticsModeSettingIndex", model);
    }

    [HttpPost]
    [Route("/admin/logistics/logistics/mode/setting/list")]
    public IActionResult List(LogisticsModeSettingListQueryParam queryParam) {
        int totalCount = GetQueryOver(queryParam)
            // .Select(Projections.Count<LogisticsModeSettingInfo>(e => logisticsModeSettingAlias!.Id))
            .Select(Projections.RowCount())
            .SingleOrDefault<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        var listQuery = GetQueryOver(queryParam);
        if (listPager.IsAscending) {
            listQuery = listQuery.OrderBy(Projections.Property(listPager.SortField)).Asc;
        } else {
            listQuery = listQuery.OrderBy(Projections.Property(listPager.SortField)).Desc;
        }

        listQuery = listQuery.ThenByAlias(() => logisticsModeSettingAlias!.Id).Desc;
            
        IList<LogisticsModeSettingInfo> logisticsModeSettingList = listQuery
            .Skip(listPager.Skip())
            .Take(listPager.Take())
            .List<LogisticsModeSettingInfo>();

        /*
        foreach (LogisticsModeSettingInfo logisticsModeSetting in logisticsModeSettingList) {
        }
        */

        LogisticsModeSettingListModel model = new LogisticsModeSettingListModel(listPager);
        model.LogisticsModeSettingList = logisticsModeSettingList;
        
        model.LogisticsModeSettingAlias = nameof(logisticsModeSettingAlias);        

        return View("LogisticsModeSettingList", model);
    }

    private LogisticsModeSettingInfo? logisticsModeSettingAlias = null;

    [NonAction]
    private IQueryOver<LogisticsModeSettingInfo, LogisticsModeSettingInfo> GetQueryOver(LogisticsModeSettingListQueryParam queryParam) {
        var query = session.QueryOver<LogisticsModeSettingInfo>(() => logisticsModeSettingAlias!);

        return query;
    }

    [HttpPost]
    [Route("/admin/logistics/logistics/mode/setting/detail")]
    public IActionResult Detail(IdParam param) {
        var id = param.Id;

        LogisticsModeSettingInfo? logisticsModeSetting;

        if (id <= 0) {
            logisticsModeSetting = new LogisticsModeSettingInfo();
        } else {
            logisticsModeSetting = session.Get<LogisticsModeSettingInfo>(id);
            if (logisticsModeSetting == null) {
                throw new ValidationException($"指定的 物流配置#{id} 不存在！");
            }
        }

        LogisticsModeSettingDetailModel model = new LogisticsModeSettingDetailModel();
        model.LogisticsModeSettingId = logisticsModeSetting.Id;

        model.VolumeWeightDenominator = logisticsModeSetting.VolumeWeightDenominator;
        model.HasFreePackageLimit = logisticsModeSetting.HasFreePackageLimit;
        model.FreePackageLimit = logisticsModeSetting.FreePackageLimit;
        model.ExtraPackageFee = logisticsModeSetting.ExtraPackageFee;
        model.HasExtraPackageFeeCapAmount = logisticsModeSetting.HasExtraPackageFeeCapAmount;
        model.ExtraPackageFeeCapAmount = logisticsModeSetting.ExtraPackageFeeCapAmount;


        return View("LogisticsModeSettingDetail", model);
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
            if (logisticsModeSetting == null){
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

    [HttpPost]
    [Route("/admin/logistics/logistics/mode/setting/delete")]
    [Transactional]
    public IActionResult Delete(IdListParam param) {
        param.Validate("请选择要删除的物流配置！");

        var idList = param.IdList;

        foreach (int id in idList) {
            LogisticsModeSettingInfo logisticsModeSetting = session.Load<LogisticsModeSettingInfo>(id);
            session.Delete(logisticsModeSetting);
        }

        session.Flush();

        return Json(new JsonResultData {
            Success = true,
            Message = "删除成功！",
        });
    }
}