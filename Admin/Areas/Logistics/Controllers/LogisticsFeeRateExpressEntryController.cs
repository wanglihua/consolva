using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Logistics;
using shunda.Admin.Areas.Logistics.Models.LogisticsFeeRateExpressEntry;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NLog;
using shunda.Domain.Pass.PassRes;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Logistics.Controllers;

/// <summary>
/// 快递具体费率明细 控制器
/// </summary>
[LogisticsArea]
[AdminAuth(PassRes.Logistics.LogisticsModeList)]
public class LogisticsFeeRateExpressEntryController : Controller {
    private static Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;

    public LogisticsFeeRateExpressEntryController(ISession session) {
        this.session = session;
    }

    [HttpGet]
    [Route("/admin/logistics/logistics/fee/rate/express/entry/{feeRateExpressId:int}")]
    public IActionResult Index(int feeRateExpressId) {
        var feeRateExpress = session.Get<LogisticsFeeRateExpressInfo>(feeRateExpressId);
        if (feeRateExpress == null) {
            throw new ValidationException($"指定的快递具体费率#{feeRateExpressId}不存在！");
        }

        NHibernateUtil.Initialize(feeRateExpress.LogisticsMode);

        LogisticsFeeRateExpressEntryIndexModel model = new LogisticsFeeRateExpressEntryIndexModel {
            FeeRateExpress = feeRateExpress,
            LogisticsMode = feeRateExpress.LogisticsMode,
        };

        return View("LogisticsFeeRateExpressEntryIndex", model);
    }

    [HttpPost]
    [Route("/admin/logistics/logistics/fee/rate/express/entry/list")]
    public IActionResult List(LogisticsFeeRateExpressEntryListQueryParam queryParam) {
        var listQuery = GetQueryOver(queryParam);


        listQuery = listQuery.ThenByAlias(() => logisticsFeeRateExpressEntryAlias!.Id).Desc;

        IList<LogisticsFeeRateExpressEntryInfo> entryList = listQuery
            .List<LogisticsFeeRateExpressEntryInfo>();

        /*
        foreach (LogisticsFeeRateExpressEntryInfo logisticsFeeRateExpressEntry in logisticsFeeRateExpressEntryList) {
        }
        */

        LogisticsFeeRateExpressEntryListModel model = new LogisticsFeeRateExpressEntryListModel {
            EntryList = entryList,
            FeeRateExpressEntryAlias = nameof(logisticsFeeRateExpressEntryAlias),
            FeeRateExpressAlias = nameof(feeRateExpressAlias)
        };

        return View("LogisticsFeeRateExpressEntryList", model);
    }

    private LogisticsFeeRateExpressEntryInfo? logisticsFeeRateExpressEntryAlias = null;
    private LogisticsFeeRateExpressInfo? feeRateExpressAlias = null;

    [NonAction]
    private IQueryOver<LogisticsFeeRateExpressEntryInfo, LogisticsFeeRateExpressEntryInfo> GetQueryOver(LogisticsFeeRateExpressEntryListQueryParam queryParam) {
        var query = session.QueryOver<LogisticsFeeRateExpressEntryInfo>(() => logisticsFeeRateExpressEntryAlias!)
            .JoinAlias(() => logisticsFeeRateExpressEntryAlias!.FeeRateExpress, () => feeRateExpressAlias);

        return query;
    }

    [HttpPost]
    [Route("/admin/logistics/logistics/fee/rate/express/entry/detail")]
    public IActionResult Detail(IdParam param) {
        if (!ModelState.IsValid) {
            throw new ValidationException(ValidationUtil.GetErrorMessages(this.ModelState));
        }

        var id = param.Id;

        LogisticsFeeRateExpressEntryInfo? feeRateExpressEntry;

        if (id <= 0) {
            feeRateExpressEntry = new LogisticsFeeRateExpressEntryInfo();
        } else {
            feeRateExpressEntry = session.Get<LogisticsFeeRateExpressEntryInfo>(id);
            if (feeRateExpressEntry == null) {
                throw new ValidationException($"指定的 快递具体费率明细#{id} 不存在！");
            }
        }

        LogisticsFeeRateExpressEntryDetailModel model = new LogisticsFeeRateExpressEntryDetailModel();
        model.FeeRateExpressEntryId = feeRateExpressEntry.Id;

        model.FeeRateExpressId = feeRateExpressEntry.FeeRateExpress?.Id ?? 0;
        model.BeginWeight = feeRateExpressEntry.BeginWeight;
        model.EndWeight = feeRateExpressEntry.EndWeight;
        model.FeeByKilogram = feeRateExpressEntry.FeeByKilogram;


        return View("LogisticsFeeRateExpressEntryDetail", model);
    }

    [HttpPost]
    [Route("/admin/logistics/logistics/fee/rate/express/entry/save")]
    [Transactional]
    public IActionResult Save(LogisticsFeeRateExpressEntrySaveModel model) {
        ValidationUtil.Validate<LogisticsFeeRateExpressEntrySaveModelValidator, LogisticsFeeRateExpressEntrySaveModel>(model);

        int logisticsFeeRateExpressEntryId = model.FeeRateExpressEntryId;

        bool isCreate = logisticsFeeRateExpressEntryId <= 0;

        LogisticsFeeRateExpressEntryInfo logisticsFeeRateExpressEntry;

        if (isCreate) {
            logisticsFeeRateExpressEntry = new LogisticsFeeRateExpressEntryInfo();
        } else {
            logisticsFeeRateExpressEntry = session.Get<LogisticsFeeRateExpressEntryInfo>(logisticsFeeRateExpressEntryId);
            if (logisticsFeeRateExpressEntry == null) {
                throw new ValidationException($"指定的 快递具体费率明细#{logisticsFeeRateExpressEntryId} 不存在！");
            }
        }

        LogisticsFeeRateExpressInfo feeRateExpress = session.Get<LogisticsFeeRateExpressInfo>(model.FeeRateExpressId);
        logisticsFeeRateExpressEntry.FeeRateExpress = feeRateExpress;
        logisticsFeeRateExpressEntry.BeginWeight = model.BeginWeight;
        logisticsFeeRateExpressEntry.EndWeight = model.EndWeight;
        logisticsFeeRateExpressEntry.FeeByKilogram = model.FeeByKilogram;

        if (isCreate) {
            session.Save(logisticsFeeRateExpressEntry);
        } else {
            session.Update(logisticsFeeRateExpressEntry);
        }

        session.Flush();

        return Json(new JsonResultData {
            Success = true,
            Message = "保存成功！",
        });
    }

    [HttpPost]
    [Route("/admin/logistics/logistics/fee/rate/express/entry/delete")]
    [Transactional]
    public IActionResult Delete(IdListParam param) {
        param.Validate("请选择要删除的快递具体费率明细！");

        var idList = param.IdList;

        foreach (int id in idList) {
            LogisticsFeeRateExpressEntryInfo logisticsFeeRateExpressEntry = session.Load<LogisticsFeeRateExpressEntryInfo>(id);
            session.Delete(logisticsFeeRateExpressEntry);
        }

        session.Flush();

        return Json(new JsonResultData {
            Success = true,
            Message = "删除成功！",
        });
    }
}