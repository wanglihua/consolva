using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using shunda.Admin.Areas.Fee.Models.ManualChargeReason;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Customer;
using shunda.Domain.Pass.PassRes;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Fee.Controllers;

[OrderFeeArea]
[AdminAuth(PassRes.Fee.ManualChargeReason)]
public class ManualChargeReasonController : Controller
{
    private readonly ISession session;

    public ManualChargeReasonController(ISession session)
    {
        this.session = session;
    }

    [HttpGet]
    [Route("/admin/manual/charge/reason")]
    public IActionResult Index()
    {
        var model = new ManualChargeReasonIndexModel();
        return View("ManualChargeReasonIndex", model);
    }

    [HttpPost]
    [Route("/admin/manual/charge/reason/list")]
    public IActionResult List(ManualChargeReasonListQueryParam queryParam)
    {
        var totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        var listPager = new ListPager(queryParam, totalCount);

        var manualChargeReasonList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<ManualChargeReasonInfo>();

        var model = new ManualChargeReasonListModel(listPager);
        model.ManualChargeReasonList = manualChargeReasonList;

        return View("ManualChargeReasonList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(ManualChargeReasonListQueryParam queryParam)
    {
        var criteria = session.CreateCriteria<ManualChargeReasonInfo>();

        criteria.SetReadOnly(true);

        return criteria;
    }

    [HttpPost]
    [Route("/admin/manual/charge/reason/detail/{id:int?}")]
    public IActionResult Detail(int? id)
    {
        session.DefaultReadOnly = true;

        ManualChargeReasonInfo manualChargeReason;

        if (id == null || id.Value == 0)
        {
            manualChargeReason = new ManualChargeReasonInfo();
        }
        else
        {
            manualChargeReason = session.Get<ManualChargeReasonInfo>(id.Value);
            if (manualChargeReason == null) throw new ValidationException("指定的数据项不存在！");
        }

        var model = new ManualChargeReasonDetailModel();
        model.ManualChargeReason = manualChargeReason;
        model.ReasonStatusList = ManualChargeReasonStatus.GetList();

        return View("ManualChargeReasonDetail", model);
    }

    [HttpPost]
    [Route("/admin/manual/charge/reason/save")]
    [Transactional]
    public IActionResult Save(ManualChargeReasonDetailModel model)
    {
        if (!ModelState.IsValid) throw new ValidationException(ValidationUtil.GetErrorMessages(ModelState));

        var manualChargeReasonFromUi = model.ManualChargeReason;
        var manualChargeReasonId = manualChargeReasonFromUi.Id;

        var isCreate = manualChargeReasonId == 0;

        ManualChargeReasonInfo manualChargeReason;

        if (isCreate)
            manualChargeReason = new ManualChargeReasonInfo();
        else
            manualChargeReason = session.Get<ManualChargeReasonInfo>(manualChargeReasonId);

        manualChargeReason.Content = manualChargeReasonFromUi.Content;
        manualChargeReason.Status = manualChargeReasonFromUi.Status;

        if (isCreate)
            session.Save(manualChargeReason);
        else
            session.Update(manualChargeReason);

        session.Flush();

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "message", "保存成功！" }
        });
    }

    [HttpPost]
    [Route("/admin/manual/charge/reason/delete")]
    [Transactional]
    public IActionResult Delete(int[] idList)
    {
        foreach (var id in idList)
        {
            var manualChargeReason = session.Load<ManualChargeReasonInfo>(id);
            session.Delete(manualChargeReason);
        }

        session.Flush();

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "message", "删除成功！" }
        });
    }
}