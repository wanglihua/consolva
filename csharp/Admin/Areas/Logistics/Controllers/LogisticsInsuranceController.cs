using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using shunda.Admin.Areas.Logistics.Models.LogisticsInsurance;
using shunda.Core;
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
    private ISession session;

    public LogisticsInsuranceController(ISession session)
    {
        this.session = session;
    }

    [HttpGet]
    [Route("/admin/logistics/insurance")]
    public IActionResult Index()
    {
        LogisticsInsuranceIndexModel model = new LogisticsInsuranceIndexModel();
        return View("LogisticsInsuranceIndex", model);
    }

    [HttpPost]
    [Route("/admin/logistics/insurance/list")]
    public IActionResult List(LogisticsInsuranceListQueryParam queryParam)
    {
        int totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<LogisticsInsuranceInfo> logisticsInsuranceList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<LogisticsInsuranceInfo>();

        LogisticsInsuranceListModel model = new LogisticsInsuranceListModel(listPager);
        model.LogisticsInsuranceList = logisticsInsuranceList;

        return View("LogisticsInsuranceList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(LogisticsInsuranceListQueryParam queryParam)
    {
        ICriteria criteria = session.CreateCriteria<LogisticsInsuranceInfo>();

        criteria = criteria.SetReadOnly(true);

        if (string.IsNullOrWhiteSpace(queryParam.NameQuery) == false)
        {
            string nameQuery = queryParam.NameQuery.Trim();
            criteria = criteria.Add(Restrictions.On<LogisticsInsuranceInfo>(e => e.Name).IsInsensitiveLike(nameQuery, MatchMode.Anywhere));
        }

        return criteria;
    }

    [HttpGet]
    [Route("/admin/logistics/insurance/detail/{id:int?}")]
    public IActionResult Detail(int? id)
    {
        LogisticsInsuranceInfo logisticsInsurance;

        if (id == null || id.Value == 0)
        {
            logisticsInsurance = new LogisticsInsuranceInfo();
        }
        else
        {
            logisticsInsurance = session.Get<LogisticsInsuranceInfo>(id.Value);
            if (logisticsInsurance == null)
            {
                return Content("指定的数据项不存在！");
            }
        }

        LogisticsInsuranceDetailModel model = new LogisticsInsuranceDetailModel();
        model.LogisticsInsurance = logisticsInsurance;

        model.FeeCalcBaseTypeList = FeeCalcBaseType.GetList();
        model.LogisticsInsuranceTypeList = LogisticsInsuranceType.GetList();
        model.LogisticsInsuranceStatusList = LogisticsInsuranceStatus.GetList();

        return View("LogisticsInsuranceDetail", model);
    }

    [HttpPost]
    [Route("/admin/logistics/insurance/save")]
    public IActionResult Save(LogisticsInsuranceDetailModel model)
    {
        if (!ModelState.IsValid)
        {
            throw new ValidationException(ValidationUtil.GetErrorMessages(this.ModelState));
        }

        LogisticsInsuranceInfo logisticsInsuranceFromUi = model.LogisticsInsurance;
        int logisticsInsuranceId = logisticsInsuranceFromUi.Id;

        bool isCreate = logisticsInsuranceId == 0;

        LogisticsInsuranceInfo logisticsInsurance;

        if (isCreate)
        {
            logisticsInsurance = new LogisticsInsuranceInfo();
        }
        else
        {
            logisticsInsurance = session.Get<LogisticsInsuranceInfo>(logisticsInsuranceId);
        }

        logisticsInsurance.Name = logisticsInsuranceFromUi.Name;
        logisticsInsurance.Content = logisticsInsuranceFromUi.Content;
        logisticsInsurance.CalcBaseType = logisticsInsuranceFromUi.CalcBaseType;
        logisticsInsurance.Type = logisticsInsuranceFromUi.Type;
        logisticsInsurance.FeeRate = logisticsInsuranceFromUi.FeeRate;
        logisticsInsurance.OrderNum = logisticsInsuranceFromUi.OrderNum;
        logisticsInsurance.Status = logisticsInsuranceFromUi.Status;

        if (isCreate)
        {
            session.Save(logisticsInsurance);
        }
        else
        {
            session.Update(logisticsInsurance);
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "保存成功！" },
            { "create", isCreate },
            { "id", logisticsInsurance.Id }
        });
    }

    [HttpPost]
    [Route("/admin/logistics/insurance/delete")]
    public IActionResult Delete(int[] idList)
    {
        foreach (int id in idList)
        {
            LogisticsInsuranceInfo logisticsInsurance = session.Load<LogisticsInsuranceInfo>(id);
            session.Delete(logisticsInsurance);
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "删除成功！" },
        });
    }
}