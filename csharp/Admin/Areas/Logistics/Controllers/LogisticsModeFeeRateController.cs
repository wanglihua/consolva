using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Logistics;
using shunda.Admin.Areas.Logistics.Models.LogisticsModeFeeRate;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NLog;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Logistics.Controllers;

/// <summary>
/// 物流方式通用费率 控制器
/// </summary>
[LogisticsArea]
// [LogisticsAuth(PassRes.XXX.LogisticsModeFeeRate)]
public class LogisticsModeFeeRateController : Controller
{
    private static Logger log = LogManager.GetCurrentClassLogger();

    private ISession session;

    public LogisticsModeFeeRateController(ISession session)
    {
        this.session = session;
    }

    [HttpGet]
    [Route("/shop/logistics/logistics/mode/fee/rate")]
    public IActionResult Index()
    {
        LogisticsModeFeeRateIndexModel model = new LogisticsModeFeeRateIndexModel();

        model.GoodsTypeList = LogisticsGoodsType.GetList();

        return View("LogisticsModeFeeRateIndex", model);
    }

    [HttpPost]
    [Route("/shop/logistics/logistics/mode/fee/rate/list")]
    public IActionResult List(LogisticsModeFeeRateListQueryParam queryParam)
    {
        int totalCount = GetQueryOver(queryParam)
            .Select(Projections.Count<LogisticsModeFeeRateInfo>(e => logisticsModeFeeRateAlias!.Id))
            .SingleOrDefault<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        var listQuery = GetQueryOver(queryParam);
        if (listPager.IsAscending)
        {
            listQuery = listQuery.OrderBy(Projections.Property(listPager.SortField)).Asc;
        }
        else
        {
            listQuery = listQuery.OrderBy(Projections.Property(listPager.SortField)).Desc;
        }

        listQuery = listQuery.ThenByAlias(() => logisticsModeFeeRateAlias!.Id).Desc;
            
        IList<LogisticsModeFeeRateInfo> logisticsModeFeeRateList = listQuery
            .Skip(listPager.Skip())
            .Take(listPager.Take())
            .List<LogisticsModeFeeRateInfo>();

        /*
        foreach (LogisticsModeFeeRateInfo logisticsModeFeeRate in logisticsModeFeeRateList)
        {
        }
        */

        LogisticsModeFeeRateListModel model = new LogisticsModeFeeRateListModel(listPager);
        model.LogisticsModeFeeRateList = logisticsModeFeeRateList;
        
        model.LogisticsModeFeeRateAlias = nameof(logisticsModeFeeRateAlias);
        model.ModeAlias = nameof(modeAlias);
        model.FeeRateAlias = nameof(feeRateAlias);        

        return View("LogisticsModeFeeRateList", model);
    }

    private LogisticsModeFeeRateInfo? logisticsModeFeeRateAlias = null;
    private LogisticsModeInfo? modeAlias = null;
    private LogisticsFeeRateInfo? feeRateAlias = null;

    [NonAction]
    private IQueryOver<LogisticsModeFeeRateInfo, LogisticsModeFeeRateInfo> GetQueryOver(LogisticsModeFeeRateListQueryParam queryParam)
    {
        var query = session.QueryOver<LogisticsModeFeeRateInfo>(() => logisticsModeFeeRateAlias!)
            .JoinAlias(() => logisticsModeFeeRateAlias!.Mode, () => modeAlias)
            .JoinAlias(() => logisticsModeFeeRateAlias!.FeeRate, () => feeRateAlias);

        return query;
    }

    [HttpPost]
    [Route("/shop/logistics/logistics/mode/fee/rate/detail")]
    public IActionResult Detail(IdParam param)
    {
        if (!ModelState.IsValid)
        {
            throw new ValidationException(ValidationUtil.GetErrorMessages(this.ModelState));
        }

        var id = param.Id;

        LogisticsModeFeeRateInfo logisticsModeFeeRate;

        if (id <= 0)
        {
            logisticsModeFeeRate = new LogisticsModeFeeRateInfo();
        }
        else
        {
            logisticsModeFeeRate = session.Get<LogisticsModeFeeRateInfo>(id);
            if (logisticsModeFeeRate == null)
            {
                throw new ValidationException($"指定的 物流方式通用费率#{id} 不存在！");
            }
        }

        LogisticsModeFeeRateDetailModel model = new LogisticsModeFeeRateDetailModel();
        model.LogisticsModeFeeRateId = logisticsModeFeeRate.Id;

        model.ModeId = logisticsModeFeeRate.Mode?.Id ?? 0;
        model.GoodsType = logisticsModeFeeRate.GoodsType;
        model.FeeRateId = logisticsModeFeeRate.FeeRate?.Id ?? 0;

        model.GoodsTypeList = LogisticsGoodsType.GetList();

        return View("LogisticsModeFeeRateDetail", model);
    }

    [HttpPost]
    [Route("/shop/logistics/logistics/mode/fee/rate/save")]
    [Transactional]
    public IActionResult Save(LogisticsModeFeeRateDetailModel model)
    {
        if (!ModelState.IsValid)
        {
            throw new ValidationException(ValidationUtil.GetErrorMessages(this.ModelState));
        }

        int logisticsModeFeeRateId = model.LogisticsModeFeeRateId;

        bool isCreate = logisticsModeFeeRateId <= 0;

        LogisticsModeFeeRateInfo logisticsModeFeeRate;

        if (isCreate)
        {
            logisticsModeFeeRate = new LogisticsModeFeeRateInfo();
        }
        else
        {
            logisticsModeFeeRate = session.Get<LogisticsModeFeeRateInfo>(logisticsModeFeeRateId);
            if (logisticsModeFeeRate == null)
            {
                throw new ValidationException($"指定的 物流方式通用费率#{logisticsModeFeeRateId} 不存在！");
            }
        }

        LogisticsModeInfo mode = session.Get<LogisticsModeInfo>(model.ModeId);
        LogisticsFeeRateInfo feeRate = session.Get<LogisticsFeeRateInfo>(model.FeeRateId);
        logisticsModeFeeRate.Mode = mode;
        logisticsModeFeeRate.GoodsType = model.GoodsType;
        logisticsModeFeeRate.FeeRate = feeRate;

        if (isCreate)
        {
            session.Save(logisticsModeFeeRate);
        }
        else
        {
            session.Update(logisticsModeFeeRate);
        }

        session.Flush();

        return Json(new {
            success = true,
            message = "保存成功！",
        });
    }

    [HttpPost]
    [Route("/shop/logistics/logistics/mode/fee/rate/delete")]
    [Transactional]
    public IActionResult Delete(IdListParam param)
    {
        param.Validate("请选择要删除的物流方式通用费率！");

        var idList = param.IdList;

        foreach (int id in idList)
        {
            LogisticsModeFeeRateInfo logisticsModeFeeRate = session.Load<LogisticsModeFeeRateInfo>(id);
            session.Delete(logisticsModeFeeRate);
        }

        session.Flush();

        return Json(new {
            success = true,
            message = "删除成功！",
        });
    }
}