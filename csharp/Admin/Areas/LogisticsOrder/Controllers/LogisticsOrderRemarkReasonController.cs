using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using shunda.Admin.Areas.LogisticsOrder.Models.LogisticsOrderRemarkReason;
using shunda.Core;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.LogisticsOrder;
using shunda.Domain.Pass.PassRes;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.LogisticsOrder.Controllers;

[LogisticsOrderArea]
[AdminAuth(PassRes.SystemSetting.LogisticsOrderRemarkReason)]
public class LogisticsOrderRemarkReasonController : Controller
{
    private ISession session = null;

    public LogisticsOrderRemarkReasonController(ISession session)
    {
        this.session = session;
    }

    [HttpGet]
    [Route("/admin/logistics/order/remark/reason")]
    public IActionResult Index()
    {
        LogisticsOrderRemarkReasonIndexModel model = new LogisticsOrderRemarkReasonIndexModel();
        return View("LogisticsOrderRemarkReasonIndex", model);
    }

    [HttpPost]
    [Route("/admin/logistics/order/remark/reason/list")]
    public IActionResult List(LogisticsOrderRemarkReasonListQueryParam queryParam)
    {
        int totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<LogisticsOrderRemarkReasonInfo> logisticsOrderRemarkReasonList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<LogisticsOrderRemarkReasonInfo>();

        foreach (LogisticsOrderRemarkReasonInfo logisticsOrderRemarkReasonInfo in logisticsOrderRemarkReasonList)
        {
            // logisticsOrderRemarkReasonInfo
        }

        LogisticsOrderRemarkReasonListModel model = new LogisticsOrderRemarkReasonListModel(listPager);
        model.LogisticsOrderRemarkReasonList = logisticsOrderRemarkReasonList;

        return View("LogisticsOrderRemarkReasonList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(LogisticsOrderRemarkReasonListQueryParam queryParam)
    {
        string reasonQuery = queryParam.ReasonQuery;
        string descriptionQuery = queryParam.DescriptionQuery;

        ICriteria criteria = session.CreateCriteria<LogisticsOrderRemarkReasonInfo>();

        criteria = criteria.SetReadOnly(true);

        if (string.IsNullOrWhiteSpace(reasonQuery) == false)
        {
            reasonQuery = reasonQuery.Trim();
            criteria = criteria.Add(Restrictions.On<LogisticsOrderRemarkReasonInfo>(e => e.Reason).IsInsensitiveLike(reasonQuery, MatchMode.Anywhere));
        }

        if (string.IsNullOrWhiteSpace(descriptionQuery) == false)
        {
            descriptionQuery = descriptionQuery.Trim();
            criteria = criteria.Add(Restrictions.On<LogisticsOrderRemarkReasonInfo>(e => e.Description).IsInsensitiveLike(descriptionQuery, MatchMode.Anywhere));
        }

        return criteria;
    }

    [HttpPost]
    [Route("/admin/logistics/order/remark/reason/detail/{id:int?}")]
    public IActionResult Detail(int? id)
    {
        session.DefaultReadOnly = true;

        LogisticsOrderRemarkReasonInfo logisticsOrderRemarkReason = null;

        if (id == null || id.Value == 0)
        {
            logisticsOrderRemarkReason = new LogisticsOrderRemarkReasonInfo();
        }
        else
        {
            logisticsOrderRemarkReason = session.Get<LogisticsOrderRemarkReasonInfo>(id.Value);
            if (logisticsOrderRemarkReason == null)
            {
                throw new ValidationException("指定的数据项不存在！");
            }
        }

        LogisticsOrderRemarkReasonDetailModel model = new LogisticsOrderRemarkReasonDetailModel();
        model.LogisticsOrderRemarkReason = logisticsOrderRemarkReason;

        return View("LogisticsOrderRemarkReasonDetail", model);
    }

    [HttpPost]
    [Route("/admin/logistics/order/remark/reason/save")]
    public IActionResult Save(LogisticsOrderRemarkReasonDetailModel model)
    {
        if (!ModelState.IsValid)
        {
            throw new ValidationException(ValidationUtil.GetErrorMessages(ModelState));
        }

        LogisticsOrderRemarkReasonInfo logisticsOrderRemarkReasonFromUi = model.LogisticsOrderRemarkReason;
        int logisticsOrderRemarkReasonId = logisticsOrderRemarkReasonFromUi.Id;

        bool isCreate = logisticsOrderRemarkReasonId == 0;

        LogisticsOrderRemarkReasonInfo logisticsOrderRemarkReason = null;

        if (isCreate)
        {
            logisticsOrderRemarkReason = new LogisticsOrderRemarkReasonInfo();
        }
        else
        {
            logisticsOrderRemarkReason = session.Get<LogisticsOrderRemarkReasonInfo>(logisticsOrderRemarkReasonId);
        }

        logisticsOrderRemarkReason.Reason = logisticsOrderRemarkReasonFromUi.Reason;
        logisticsOrderRemarkReason.Description = logisticsOrderRemarkReasonFromUi.Description;

        if (isCreate)
        {
            session.Save(logisticsOrderRemarkReason);
        }
        else
        {
            session.Update(logisticsOrderRemarkReason);
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "保存成功！" }
        });
    }

    [HttpPost]
    [Route("/admin/logistics/order/remark/reason/delete")]
    public IActionResult Delete(int[] idList)
    {
        foreach (int id in idList)
        {
            LogisticsOrderRemarkReasonInfo logisticsOrderRemarkReason = session.Load<LogisticsOrderRemarkReasonInfo>(id);
            session.Delete(logisticsOrderRemarkReason);
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "删除成功！" },
        });
    }
}