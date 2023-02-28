using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NLog;
using shunda.Admin.Areas.Logistics.Models.LogisticsGoodsTypeSetting;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Logistics;
using shunda.Domain.Pass.PassRes;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Logistics.Controllers;

/// <summary>
/// 物品类型介绍 控制器
/// </summary>
[LogisticsArea]
[AdminAuth(PassRes.Goods.GoodsTrade, PassRes.Goods.GoodsListByType)]
public class LogisticsGoodsTypeSettingController : Controller {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;

    public LogisticsGoodsTypeSettingController(ISession session) {
        this.session = session;
    }

    [HttpGet]
    [Route("/admin/logistics/logistics/goods/type/setting")]
    [Transactional]
    public IActionResult Index() {
        LogisticsGoodsTypeSettingIndexModel model = new LogisticsGoodsTypeSettingIndexModel();

        var goodsTypeList = LogisticsGoodsType.GetInnerList();

        model.TypeList = goodsTypeList;
        model.StatusList = LogisticsGoodsTypeStatus.GetList();

        var settingList = session.Query<LogisticsGoodsTypeSettingInfo>().ToList();

        foreach (LogisticsGoodsType goodsType in goodsTypeList) {
            var setting = settingList.Where(e => e.Type == goodsType).Take(1).SingleOrDefault();
            if (setting == null) {
                setting = new LogisticsGoodsTypeSettingInfo {
                    Type = goodsType,
                    Description = string.Empty,
                    SampleGoods = string.Empty,
                    Status = LogisticsGoodsTypeStatus.Enabled,
                };

                session.Save(setting);
            }
        }

        session.Flush();

        return View("LogisticsGoodsTypeSettingIndex", model);
    }

    [HttpPost]
    [Route("/admin/logistics/logistics/goods/type/setting/list")]
    public IActionResult List(LogisticsGoodsTypeSettingListQueryParam queryParam) {
        var listQuery = GetQueryOver(queryParam);
        listQuery = listQuery.OrderBy(e => e.Id).Asc;


        listQuery = listQuery.ThenByAlias(() => logisticsGoodsTypeSettingAlias!.Id).Desc;

        IList<LogisticsGoodsTypeSettingInfo> logisticsGoodsTypeSettingList = listQuery
            .List<LogisticsGoodsTypeSettingInfo>();

        /*
        foreach (LogisticsGoodsTypeSettingInfo logisticsGoodsTypeSetting in logisticsGoodsTypeSettingList)
        {
        }
        */

        LogisticsGoodsTypeSettingListModel model = new LogisticsGoodsTypeSettingListModel {
            LogisticsGoodsTypeSettingList = logisticsGoodsTypeSettingList,
            LogisticsGoodsTypeSettingAlias = nameof(logisticsGoodsTypeSettingAlias),
        };

        return View("LogisticsGoodsTypeSettingList", model);
    }

    private LogisticsGoodsTypeSettingInfo? logisticsGoodsTypeSettingAlias = null;

    [NonAction]
    private IQueryOver<LogisticsGoodsTypeSettingInfo, LogisticsGoodsTypeSettingInfo> GetQueryOver(LogisticsGoodsTypeSettingListQueryParam queryParam) {
        var query = session.QueryOver<LogisticsGoodsTypeSettingInfo>(() => logisticsGoodsTypeSettingAlias!);

        return query;
    }

    [HttpPost]
    [Route("/admin/logistics/logistics/goods/type/setting/detail")]
    public IActionResult Detail(IdParam param) {
        if (!ModelState.IsValid) {
            throw new ValidationException(ValidationUtil.GetErrorMessages(this.ModelState));
        }

        var id = param.Id;

        LogisticsGoodsTypeSettingInfo? logisticsGoodsTypeSetting;

        if (id <= 0) {
            logisticsGoodsTypeSetting = new LogisticsGoodsTypeSettingInfo();
        } else {
            logisticsGoodsTypeSetting = session.Get<LogisticsGoodsTypeSettingInfo>(id);
            if (logisticsGoodsTypeSetting == null) {
                throw new ValidationException($"指定的 物品类型介绍#{id} 不存在！");
            }
        }

        LogisticsGoodsTypeSettingDetailModel model = new LogisticsGoodsTypeSettingDetailModel();
        model.LogisticsGoodsTypeSettingId = logisticsGoodsTypeSetting.Id;

        model.Type = logisticsGoodsTypeSetting.Type;
        model.Description = logisticsGoodsTypeSetting.Description;
        model.SampleGoods = logisticsGoodsTypeSetting.SampleGoods;
        model.Status = logisticsGoodsTypeSetting.Status;

        model.TypeList = LogisticsGoodsType.GetInnerList();
        model.StatusList = LogisticsGoodsTypeStatus.GetList();

        return View("LogisticsGoodsTypeSettingDetail", model);
    }

    [HttpPost]
    [Route("/admin/logistics/logistics/goods/type/setting/save")]
    [Transactional]
    public IActionResult Save(LogisticsGoodsTypeSettingSaveModel model) {
        if (!ModelState.IsValid) {
            throw new ValidationException(ValidationUtil.GetErrorMessages(this.ModelState));
        }

        int logisticsGoodsTypeSettingId = model.LogisticsGoodsTypeSettingId;

        LogisticsGoodsTypeSettingInfo logisticsGoodsTypeSetting = session.Get<LogisticsGoodsTypeSettingInfo>(logisticsGoodsTypeSettingId);
        if (logisticsGoodsTypeSetting == null) {
            throw new ValidationException($"指定的 物品类型介绍#{logisticsGoodsTypeSettingId} 不存在！");
        }

        // logisticsGoodsTypeSetting.Type = model.Type;
        logisticsGoodsTypeSetting.Description = model.Description;
        logisticsGoodsTypeSetting.SampleGoods = model.SampleGoods;
        logisticsGoodsTypeSetting.Status = model.Status;

        session.Update(logisticsGoodsTypeSetting);
        session.Flush();

        return Json(new JsonResultData {
            Success = true,
            Message = "保存成功！",
        });
    }
}