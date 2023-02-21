using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using shunda.Admin.Areas.Logistics.Models.LogisticsInsurance;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Logistics;
using shunda.Domain.Pass.PassRes;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Logistics.Controllers;

[LogisticsArea]
[AdminAuth(PassRes.Logistics.Insurance)]
public class LogisticsInsuranceController : Controller
{
    private readonly ISession session;

    public LogisticsInsuranceController(ISession session)
    {
        this.session = session;
    }

    [HttpGet]
    [Route("/admin/logistics/insurance")]
    public IActionResult Index()
    {
        var model = new LogisticsInsuranceIndexModel();
        return View("LogisticsInsuranceIndex", model);
    }

    [HttpPost]
    [Route("/admin/logistics/insurance/list")]
    public IActionResult List(LogisticsInsuranceListQueryParam queryParam)
    {
        var totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        var listPager = new ListPager(queryParam, totalCount);

        var logisticsInsuranceList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<LogisticsInsuranceInfo>();

        var model = new LogisticsInsuranceListModel(listPager);
        model.LogisticsInsuranceList = logisticsInsuranceList;

        return View("LogisticsInsuranceList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(LogisticsInsuranceListQueryParam queryParam)
    {
        var criteria = session.CreateCriteria<LogisticsInsuranceInfo>();

        criteria = criteria.SetReadOnly(true);

        if (string.IsNullOrWhiteSpace(queryParam.NameQuery) == false)
        {
            var nameQuery = queryParam.NameQuery.Trim();
            criteria = criteria.Add(Restrictions.On<LogisticsInsuranceInfo>(e => e.Name).IsInsensitiveLike(nameQuery, MatchMode.Anywhere));
        }

        return criteria;
    }

    [HttpGet]
    [Route("/admin/logistics/insurance/detail/{id:int?}")]
    public IActionResult Detail(int? id)
    {
        LogisticsInsuranceWithContentInfo? logisticsInsurance;

        if (id == null || id.Value == 0)
        {
            logisticsInsurance = new LogisticsInsuranceWithContentInfo();
        }
        else
        {
            logisticsInsurance = session.Get<LogisticsInsuranceWithContentInfo>(id.Value);
            if (logisticsInsurance == null) return Content("指定的数据项不存在！");
        }

        var model = new LogisticsInsuranceDetailModel();
        model.LogisticsInsurance = logisticsInsurance;

        model.FeeCalcBaseTypeList = FeeCalcBaseType.GetList();
        model.LogisticsInsuranceTypeList = LogisticsInsuranceType.GetList();
        model.LogisticsInsuranceStatusList = LogisticsInsuranceStatus.GetList();

        return View("LogisticsInsuranceDetail", model);
    }

    [HttpPost]
    [Route("/admin/logistics/insurance/save")]
    [Transactional]
    public IActionResult Save(LogisticsInsuranceDetailModel model)
    {
        if (!ModelState.IsValid) throw new ValidationException(ValidationUtil.GetErrorMessages(ModelState));

        var logisticsInsuranceFromUi = model.LogisticsInsurance;
        var logisticsInsuranceId = logisticsInsuranceFromUi.Id;

        var isCreate = logisticsInsuranceId == 0;

        LogisticsInsuranceWithContentInfo? logisticsInsurance;

        if (isCreate)
            logisticsInsurance = new LogisticsInsuranceWithContentInfo();
        else
            logisticsInsurance = session.Get<LogisticsInsuranceWithContentInfo>(logisticsInsuranceId);

        logisticsInsurance.Name = logisticsInsuranceFromUi.Name;
        logisticsInsurance.Content = logisticsInsuranceFromUi.Content;
        logisticsInsurance.CalcBaseType = logisticsInsuranceFromUi.CalcBaseType;
        logisticsInsurance.Type = logisticsInsuranceFromUi.Type;
        logisticsInsurance.FeeRate = logisticsInsuranceFromUi.FeeRate;
        logisticsInsurance.OrderNum = logisticsInsuranceFromUi.OrderNum;
        logisticsInsurance.Status = logisticsInsuranceFromUi.Status;

        if (isCreate)
            session.Save(logisticsInsurance);
        else
            session.Update(logisticsInsurance);

        session.Flush();

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "message", "保存成功！" },
            { "create", isCreate },
            { "id", logisticsInsurance.Id }
        });
    }

    [HttpPost]
    [Route("/admin/logistics/insurance/delete")]
    [Transactional]
    public IActionResult Delete(int[] idList)
    {
        foreach (var id in idList)
        {
            var logisticsInsurance = session.Load<LogisticsInsuranceInfo>(id);
            session.Delete(logisticsInsurance);
        }

        session.Flush();

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "message", "删除成功！" }
        });
    }
}