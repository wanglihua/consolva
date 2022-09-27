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
    private ISession session;

    public LogisticsGstController(ISession session)
    {
        this.session = session;
    }

    [HttpGet]
    [Route("/admin/logistics/gst")]
    public IActionResult Index()
    {
        LogisticsGstIndexModel model = new LogisticsGstIndexModel();
        return View("LogisticsGstIndex", model);
    }

    [HttpPost]
    [Route("/admin/logistics/gst/list")]
    public IActionResult List(LogisticsGstListQueryParam queryParam)
    {
        int totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<LogisticsGstInfo> logisticsGstList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<LogisticsGstInfo>();
        

        LogisticsGstListModel model = new LogisticsGstListModel(listPager);
        model.GstList = logisticsGstList;

        return View("LogisticsGstList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(LogisticsGstListQueryParam queryParam)
    {
        ICriteria criteria = session.CreateCriteria<LogisticsGstInfo>();

        criteria = criteria.SetReadOnly(true);

        if (string.IsNullOrWhiteSpace(queryParam.NameQuery) == false)
        {
            string nameQuery = queryParam.NameQuery.Trim();
            criteria = criteria.Add(Restrictions.On<LogisticsGstInfo>(e => e.Name).IsInsensitiveLike(nameQuery, MatchMode.Anywhere));
        }

        return criteria;
    }

    [HttpPost]
    [Route("/admin/logistics/gst/detail/{id:int?}")]
    public IActionResult Detail(int? id)
    {
        session.DefaultReadOnly = true;

        LogisticsGstInfo logisticsGst;

        if (id == null || id.Value == 0)
        {
            logisticsGst = new LogisticsGstInfo();
        }
        else
        {
            logisticsGst = session.Get<LogisticsGstInfo>(id.Value);
            if (logisticsGst == null)
            {
                throw new ValidationException("指定的数据项不存在！");
            }
        }

        LogisticsGstDetailModel model = new LogisticsGstDetailModel();
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
        if (!ModelState.IsValid)
        {
            throw new ValidationException(ValidationUtil.GetErrorMessages(this.ModelState));
        }

        LogisticsGstInfo logisticsGstFromUi = model.LogisticsGst;
        int logisticsGstId = logisticsGstFromUi.Id;

        bool isCreate = logisticsGstId == 0;

        LogisticsGstInfo logisticsGst;

        if (isCreate)
        {
            logisticsGst = new LogisticsGstInfo();
        }
        else
        {
            logisticsGst = session.Get<LogisticsGstInfo>(logisticsGstId);
        }

        logisticsGst.Name = logisticsGstFromUi.Name;
        logisticsGst.CalcBaseType = logisticsGstFromUi.CalcBaseType;
        logisticsGst.StartAmount = logisticsGstFromUi.StartAmount;
        logisticsGst.FeeRate = logisticsGstFromUi.FeeRate;
        logisticsGst.OrderNum = logisticsGstFromUi.OrderNum;
        logisticsGst.Status = logisticsGstFromUi.Status;

        if (isCreate)
        {
            session.Save(logisticsGst);
        }
        else
        {
            session.Update(logisticsGst);
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "保存成功！" }
        });
    }

    [HttpPost]
    [Route("/admin/logistics/gst/delete")]
    [Transactional]
    public IActionResult Delete(int[] idList)
    {
        foreach (int id in idList)
        {
            LogisticsGstInfo logisticsGst = session.Load<LogisticsGstInfo>(id);
            session.Delete(logisticsGst);
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "删除成功！" },
        });
    }
}