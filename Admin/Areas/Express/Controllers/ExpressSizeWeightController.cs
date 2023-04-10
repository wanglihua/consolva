using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using shunda.Admin.Areas.Express.Models.ExpressSizeWeight;
using shunda.Core;
using shunda.Core.Config;
using shunda.Core.Hibernate;
using shunda.Core.Web;
using shunda.Domain.Express;
using shunda.Domain.Pass.PassRes;
using shunda.Service.Logistics;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Express.Controllers;

[ExpressArea]
[AdminAuth(PassRes.Express.ExpressSizeWeight)]
public class ExpressSizeWeightController : Controller {
    private readonly ISession session;
    private readonly LogisticsModeSettingService logisticsModeSettingService;

    public ExpressSizeWeightController(
        ISession session,
        LogisticsModeSettingService logisticsModeSettingService
    ) {
        this.session = session;
        this.logisticsModeSettingService = logisticsModeSettingService;
    }

    [HttpGet]
    [Route("/admin/express/size/weight")]
    public IActionResult Index() {
        ExpressSizeWeightIndexModel model = new ExpressSizeWeightIndexModel();

        // 这里有个小问题，就是没计算进去子单号
        model.AvgVolumeWeightActualWeightRatio = session
            .CreateSQLQuery("SELECT ifnull(avg((le.length * le.width * le.height) / 5000 / le.weight), 0) FROM tbl_express_size_weight le WHERE le.create_time >= date_add(now(), INTERVAL -30 DAY)")
            .UniqueResult<decimal>()
            .ToString("#0.00");

        return View("ExpressSizeWeightIndex", model);
    }

    [HttpPost]
    [Route("/admin/express/size/weight/list")]
    public IActionResult List(ExpressSizeWeightListQueryParam queryParam) {
        int totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<ExpressSizeWeightInfo> expressSizeWeightList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<ExpressSizeWeightInfo>();

        ExpressSizeWeightListModel model = new ExpressSizeWeightListModel(listPager);
        model.ExpressSizeWeightList = expressSizeWeightList;
        model.VolumeWeightDenominator = logisticsModeSettingService.GetSetting().VolumeWeightDenominator;

        return View("ExpressSizeWeightList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(ExpressSizeWeightListQueryParam queryParam) {
        ICriteria criteria = session.CreateCriteria<ExpressSizeWeightInfo>();
        criteria = criteria.SetReadOnly(true);

        // criteria = criteria.CreateAlias(nameof(ExpressSizeWeightInfo.Order), nameof(ExpressSizeWeightInfo.Order), JoinType.LeftOuterJoin);
        criteria = criteria.CreateAlias(nameof(ExpressSizeWeightInfo.Machine), nameof(ExpressSizeWeightInfo.Machine), JoinType.LeftOuterJoin);
        criteria = criteria.CreateAlias(nameof(ExpressSizeWeightInfo.User), nameof(ExpressSizeWeightInfo.User), JoinType.LeftOuterJoin);

        criteria = criteria.Add(Restrictions.Where<ExpressSizeWeightInfo>(e => e.TrackingNoSub == string.Empty)); // 这里只查询主单号

        if (string.IsNullOrWhiteSpace(queryParam.TrackingNoQuery) == false) {
            string trackingNoQuery = queryParam.TrackingNoQuery.Trim();
            criteria = criteria.Add(Restrictions.Where<ExpressSizeWeightInfo>(e => e.TrackingNo == trackingNoQuery));
        }

        if (queryParam.CreateDateBeginQuery != null) {
            DateTime createDateBeginQuery = queryParam.CreateDateBeginQuery.Value;
            criteria = criteria.Add(Restrictions.Where<ExpressSizeWeightInfo>(e => e.CreateTime >= createDateBeginQuery));
        }

        if (queryParam.CreateDateEndQuery != null) {
            DateTime createDateEndQuery = queryParam.CreateDateEndQuery.Value;
            createDateEndQuery = createDateEndQuery.AddDays(1);
            criteria = criteria.Add(Restrictions.Where<ExpressSizeWeightInfo>(e => e.CreateTime < createDateEndQuery));
        }

        return criteria;
    }

    [HttpPost]
    [Route("/admin/express/size/weight/delete")]
    [Transactional]
    public IActionResult Delete(int[] idList) {
        foreach (int id in idList) {
            ExpressSizeWeightInfo logisticsExpress = session.Load<ExpressSizeWeightInfo>(id);
            session.Delete(logisticsExpress);
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "删除成功！" },
        });
    }
}