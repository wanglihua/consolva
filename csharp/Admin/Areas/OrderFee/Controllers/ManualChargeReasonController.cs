using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using shunda.Admin.Areas.OrderFee.Models.ManualChargeReason;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Domain.Customer;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.OrderFee.Controllers;

[OrderFeeArea]
// todo [AdminAuth(PassRes.SystemSetting.ManualChargeReason)]
public class ManualChargeReasonController : Controller
{
    private ISession session;

    public ManualChargeReasonController(ISession session)
    {
        this.session = session;
    }

    [HttpGet]
    [Route("/admin/manual/charge/reason")]
    public IActionResult Index()
    {
        ManualChargeReasonIndexModel model = new ManualChargeReasonIndexModel();
        return View("ManualChargeReasonIndex", model);
    }

    [HttpPost]
    [Route("/admin/manual/charge/reason/list")]
    public IActionResult List(ManualChargeReasonListQueryParam queryParam)
    {
        int totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<ManualChargeReasonInfo> manualChargeReasonList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<ManualChargeReasonInfo>();

        foreach (ManualChargeReasonInfo manualChargeReasonInfo in manualChargeReasonList)
        {
            // manualChargeReasonInfo
        }

        ManualChargeReasonListModel model = new ManualChargeReasonListModel(listPager);
        model.ManualChargeReasonList = manualChargeReasonList;

        return View("ManualChargeReasonList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(ManualChargeReasonListQueryParam queryParam)
    {
        ICriteria criteria = session.CreateCriteria<ManualChargeReasonInfo>();

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
            if (manualChargeReason == null)
            {
                throw new ValidationException("指定的数据项不存在！");
            }
        }

        ManualChargeReasonDetailModel model = new ManualChargeReasonDetailModel();
        model.ManualChargeReason = manualChargeReason;
        model.ReasonStatusList = ManualChargeReasonStatus.GetList();

        return View("ManualChargeReasonDetail", model);
    }

    [HttpPost]
    [Route("/admin/manual/charge/reason/save")]
    [Transactional]
    public IActionResult Save(ManualChargeReasonDetailModel model)
    {
        if (!ModelState.IsValid)
        {
            throw new ValidationException(ValidationUtil.GetErrorMessages(ModelState));
        }

        ManualChargeReasonInfo manualChargeReasonFromUi = model.ManualChargeReason;
        int manualChargeReasonId = manualChargeReasonFromUi.Id;

        bool isCreate = manualChargeReasonId == 0;

        ManualChargeReasonInfo manualChargeReason;

        if (isCreate)
        {
            manualChargeReason = new ManualChargeReasonInfo();
        }
        else
        {
            manualChargeReason = session.Get<ManualChargeReasonInfo>(manualChargeReasonId);
        }

        manualChargeReason.Content = manualChargeReasonFromUi.Content;
        manualChargeReason.Status = manualChargeReasonFromUi.Status;

        if (isCreate)
        {
            session.Save(manualChargeReason);
        }
        else
        {
            session.Update(manualChargeReason);
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "保存成功！" }
        });
    }

    [HttpPost]
    [Route("/admin/manual/charge/reason/delete")]
    [Transactional]
    public IActionResult Delete(int[] idList)
    {
        foreach (int id in idList)
        {
            ManualChargeReasonInfo manualChargeReason = session.Load<ManualChargeReasonInfo>(id);
            session.Delete(manualChargeReason);
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "删除成功！" },
        });
    }
}