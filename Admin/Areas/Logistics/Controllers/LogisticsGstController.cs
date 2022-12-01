using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using shunda.Admin.Areas.Logistics.Models.LogisticsGst;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Logistics;
using shunda.Domain.Pass.PassRes;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Logistics.Controllers;

[LogisticsArea]
[AdminAuth(PassRes.Logistics.Gst)]
public class LogisticsGstController : Controller
{
    private readonly ISession session;

    public LogisticsGstController(ISession session)
    {
        this.session = session;
    }

    [HttpGet]
    [Route("/admin/logistics/gst")]
    public IActionResult Index()
    {
        var model = new LogisticsGstIndexModel();
        return View("LogisticsGstIndex", model);
    }

    [HttpPost]
    [Route("/admin/logistics/gst/list")]
    public IActionResult List(LogisticsGstListQueryParam queryParam)
    {
        var totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        var listPager = new ListPager(queryParam, totalCount);

        var logisticsGstList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<LogisticsGstInfo>();


        var model = new LogisticsGstListModel(listPager);
        model.GstList = logisticsGstList;

        return View("LogisticsGstList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(LogisticsGstListQueryParam queryParam)
    {
        var criteria = session.CreateCriteria<LogisticsGstInfo>();

        criteria = criteria.SetReadOnly(true);

        if (string.IsNullOrWhiteSpace(queryParam.NameQuery) == false)
        {
            var nameQuery = queryParam.NameQuery.Trim();
            criteria = criteria.Add(Restrictions.On<LogisticsGstInfo>(e => e.Name).IsInsensitiveLike(nameQuery, MatchMode.Anywhere));
        }

        return criteria;
    }

    [HttpPost]
    [Route("/admin/logistics/gst/detail/{id:int?}")]
    public IActionResult Detail(int? id)
    {
        LogisticsGstInfo? logisticsGst;

        if (id == null || id.Value == 0)
        {
            logisticsGst = new LogisticsGstInfo();
        }
        else
        {
            logisticsGst = session.Get<LogisticsGstInfo>(id.Value);
            if (logisticsGst == null) throw new ValidationException("指定的数据项不存在！");
        }

        var model = new LogisticsGstDetailModel();
        model.LogisticsGst = logisticsGst;

        model.FeeCalcBaseTypeList = FeeCalcBaseType.GetList();
        model.GstStatusList = LogisticsGstStatus.GetList();

        return View("LogisticsGstDetail", model);
    }

    [HttpPost]
    [Route("/admin/logistics/gst/save")]
    [Transactional]
    public IActionResult Save(LogisticsGstDetailModel model)
    {
        if (!ModelState.IsValid) throw new ValidationException(ValidationUtil.GetErrorMessages(ModelState));

        var logisticsGstFromUi = model.LogisticsGst;
        var logisticsGstId = logisticsGstFromUi.Id;

        var isCreate = logisticsGstId == 0;

        LogisticsGstInfo? logisticsGst;

        if (isCreate)
            logisticsGst = new LogisticsGstInfo();
        else
            logisticsGst = session.Get<LogisticsGstInfo>(logisticsGstId);

        logisticsGst.Name = logisticsGstFromUi.Name;
        logisticsGst.CalcBaseType = logisticsGstFromUi.CalcBaseType;
        logisticsGst.StartAmount = logisticsGstFromUi.StartAmount;
        logisticsGst.FeeRate = logisticsGstFromUi.FeeRate;
        logisticsGst.OrderNum = logisticsGstFromUi.OrderNum;
        logisticsGst.Status = logisticsGstFromUi.Status;

        if (isCreate)
            session.Save(logisticsGst);
        else
            session.Update(logisticsGst);

        session.Flush();

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "message", "保存成功！" }
        });
    }

    [HttpPost]
    [Route("/admin/logistics/gst/delete")]
    [Transactional]
    public IActionResult Delete(int[] idList)
    {
        foreach (var id in idList)
        {
            var logisticsGst = session.Load<LogisticsGstInfo>(id);
            session.Delete(logisticsGst);
        }

        session.Flush();

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "message", "删除成功！" }
        });
    }
}