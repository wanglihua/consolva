using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using shunda.Admin.Areas.Logistics.Models.LogisticsModeInner;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Logistics;
using shunda.Domain.Pass.PassRes;
using shunda.Domain.Storage;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Logistics.Controllers;

/// <summary>
///     内部计费物流管理
/// </summary>
[LogisticsArea]
[AdminAuth(PassRes.Logistics.LogisticsModeList)]
public class LogisticsModeInnerController : Controller
{
    private readonly ISession session;

    public LogisticsModeInnerController(ISession session)
    {
        this.session = session;
    }

    [HttpGet]
    [Route("/admin/logistics/mode/inner/{typeId:int}")]
    public IActionResult Index(int typeId)
    {
        var model = new LogisticsModeInnerIndexModel();
        model.TypeId = typeId;
        model.Type = LogisticsModeType.GetById(typeId);

        model.LogisticsModeTypeList = LogisticsModeType.GetList();
        model.StatusList = LogisticsModeStatus.GetList();

        return View("LogisticsModeInnerIndex", model);
    }

    [HttpPost]
    [Route("/admin/logistics/mode/inner/list")]
    public IActionResult List(LogisticsModeInnerListQueryParam queryParam)
    {
        var totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        var listPager = new ListPager(queryParam, totalCount);

        var logisticsModeList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<LogisticsModeInfo>();

        var model = new LogisticsModeInnerListModel(listPager);
        model.LogisticsModeList = logisticsModeList;
        model.ModeType = LogisticsModeType.GetById(queryParam.TypeIdQuery ?? 0);

        return View("LogisticsModeInnerList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(LogisticsModeInnerListQueryParam queryParam)
    {
        var criteria = session.CreateCriteria<LogisticsModeInfo>();

        criteria = criteria.SetReadOnly(true);

        criteria = criteria.CreateAlias(nameof(LogisticsModeInfo.Warehouse), nameof(LogisticsModeInfo.Warehouse));

        criteria = criteria.Add(Restrictions.Where<LogisticsModeInfo>(e => e.Cate == LogisticsModeCate.Inner));

        if (queryParam.TypeIdQuery != null && queryParam.TypeIdQuery.Value != 0)
        {
            var typeQuery = LogisticsModeType.GetById(queryParam.TypeIdQuery.Value);
            criteria = criteria.Add(Restrictions.Where<LogisticsModeInfo>(e => e.Type == typeQuery));
        }

        if (string.IsNullOrWhiteSpace(queryParam.CodeQuery) == false)
        {
            var codeQuery = queryParam.CodeQuery.Trim();
            criteria = criteria.Add(Restrictions.On<LogisticsModeInfo>(e => e.Code).IsLike(codeQuery, MatchMode.Anywhere));
        }

        if (string.IsNullOrWhiteSpace(queryParam.NameQuery) == false)
        {
            var nameQuery = queryParam.NameQuery.Trim();
            criteria = criteria.Add(Restrictions.On<LogisticsModeInfo>(e => e.Name).IsLike(nameQuery, MatchMode.Anywhere));
        }


        if (queryParam.StatusQuery != null && queryParam.StatusQuery != LogisticsModeStatus.None)
        {
            var statusQuery = queryParam.StatusQuery;
            criteria = criteria.Add(Restrictions.Where<LogisticsModeInfo>(e => e.Status == statusQuery));
        }

        return criteria;
    }

    [HttpGet]
    [Route("/admin/logistics/mode/inner/detail/{id:int}/{typeId:int?}")]
    public IActionResult Detail(int id, int? typeId)
    {
        session.DefaultReadOnly = true;

        session.DefaultReadOnly = true;

        var typeList = LogisticsModeType.GetList();

        var model = new LogisticsModeInnerDetailModel();

        LogisticsModeInfo logisticsMode;
        if (id == 0)
        {
            logisticsMode = new LogisticsModeInfo();

            if (typeId.HasValue)
                logisticsMode.Type = LogisticsModeType.GetById(typeId.Value);
            else
                logisticsMode.Type = LogisticsModeType.GetById(typeList.First().Id);
        }
        else
        {
            logisticsMode = session.Get<LogisticsModeInfo>(id);
            if (logisticsMode == null) throw new ValidationException("指定的数据项不存在！");
        }

        model.Id = logisticsMode.Id;
        model.Type = logisticsMode.Type;
        model.Code = logisticsMode.Code;
        model.Name = logisticsMode.Name;
        model.Description = logisticsMode.Description;

        model.Status = logisticsMode.Status;

        // todo 这里有可修改

        var feeRateEnabledStatus = LogisticsFeeRateStatus.Enabled;
        model.LogisticsFeeRateList = session.Query<LogisticsFeeRateInfo>().Where(e => e.Status == feeRateEnabledStatus).OrderBy(e => e.Id).ToList();
        model.StatusList = LogisticsModeStatus.GetList();

        return View("LogisticsModeInnerDetail", model);
    }

    [HttpPost]
    [Route("/admin/logistics/mode/inner/save")]
    [Transactional]
    public IActionResult Save(LogisticsModeInnerDetailModel model)
    {
        if (!ModelState.IsValid) throw new ValidationException(ValidationUtil.GetErrorMessages(ModelState));

        var logisticsModeId = model.Id;

        var isCreate = logisticsModeId == 0;

        LogisticsModeInfo logisticsMode;

        if (isCreate)
        {
            logisticsMode = new LogisticsModeInfo();
            logisticsMode.Cate = LogisticsModeCate.Inner;
        }
        else
        {
            logisticsMode = session.Get<LogisticsModeInfo>(logisticsModeId);
        }

        logisticsMode.Type = model.Type;
        logisticsMode.Code = model.Code;
        logisticsMode.Name = model.Name;
        logisticsMode.Description = model.Description;

        logisticsMode.Status = model.Status;

        // todo 这里有可修改


        logisticsMode.Warehouse = session.Query<WarehouseInfo>().Take(1).SingleOrDefault()!; // 随便给个仓库

        if (!ModelState.IsValid) throw new ValidationException(ValidationUtil.GetErrorMessages(ModelState));

        if (isCreate)
            session.Save(logisticsMode);
        else
            session.Update(logisticsMode);

        session.Flush();

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "message", "保存成功！" },
            { "create", isCreate },
            { "id", logisticsMode.Id }
        });
    }

    [HttpPost]
    [Route("/admin/logistics/mode/inner/delete")]
    [Transactional]
    public IActionResult Delete(int[] idList)
    {
        foreach (var modeId in idList)
        {
            var logisticsMode = session.Load<LogisticsModeInfo>(modeId);

            // 先删除关联的费率数据
            if (logisticsMode.Type == LogisticsModeType.CnAuExpress) session.Query<LogisticsFeeRateExpressInfo>().Where(e => e.LogisticsModeId == modeId).Delete();

            session.Delete(logisticsMode);
        }

        session.Flush();

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "message", "删除成功！" }
        });
    }
}