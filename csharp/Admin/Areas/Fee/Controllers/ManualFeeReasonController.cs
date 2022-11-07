using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using shunda.Admin.Areas.OrderFee.Models.ManualFeeReason;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Fee;
using shunda.Domain.Order;
using shunda.Domain.Pass.PassRes;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.OrderFee.Controllers;

[OrderFeeArea]
[AdminAuth(PassRes.OrderFee.ManualFeeReason)]
public class ManualFeeReasonController : Controller
{
    private ISession session;

    public ManualFeeReasonController(ISession session)
    {
        this.session = session;
    }

    [HttpGet]
    [Route("/admin/manual/fee/reason")]
    public IActionResult Index()
    {
        ManualFeeReasonIndexModel model = new ManualFeeReasonIndexModel();
        model.FeeTypeList = ManualFeeType.GetList();
        model.StatusList = ManualFeeReasonStatus.GetList();

        return View("ManualFeeReasonIndex", model);
    }

    [HttpPost]
    [Route("/admin/manual/fee/reason/list")]
    public IActionResult List(ManualFeeReasonListQueryParam queryParam)
    {
        int totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<ManualFeeReasonInfo> manualFeeReasonList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<ManualFeeReasonInfo>();

        foreach (ManualFeeReasonInfo manualFeeReasonInfo in manualFeeReasonList)
        {
            // manualFeeReasonInfo
        }

        ManualFeeReasonListModel model = new ManualFeeReasonListModel(listPager);
        model.ManualFeeReasonList = manualFeeReasonList;

        return View("ManualFeeReasonList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(ManualFeeReasonListQueryParam queryParam)
    {
        ICriteria criteria = session.CreateCriteria<ManualFeeReasonInfo>();

        criteria = criteria.SetReadOnly(true);

        ManualFeeType? feeTypeQuery = queryParam.FeeTypeQuery;
        if (feeTypeQuery != null && feeTypeQuery != ManualFeeType.None)
        {
            criteria = criteria.Add(Restrictions.Where<ManualFeeReasonInfo>(e => e.FeeType == feeTypeQuery));
        }

        ManualFeeReasonStatus? statusQuery = queryParam.StatusQuery;
        if (statusQuery != null && statusQuery != ManualFeeReasonStatus.None)
        {
            criteria = criteria.Add(Restrictions.Where<ManualFeeReasonInfo>(e => e.Status == statusQuery));
        }

        return criteria;
    }

    [HttpPost]
    [Route("/admin/manual/fee/reason/detail/{id:int?}")]
    public IActionResult Detail(int? id)
    {
        session.DefaultReadOnly = true;

        ManualFeeReasonInfo manualFeeReason;

        if (id == null || id.Value == 0)
        {
            manualFeeReason = new ManualFeeReasonInfo();
        }
        else
        {
            manualFeeReason = session.Get<ManualFeeReasonInfo>(id.Value);
            if (manualFeeReason == null)
            {
                throw new ValidationException("指定的数据项不存在！");
            }
        }

        ManualFeeReasonDetailModel model = new ManualFeeReasonDetailModel();
        model.FeeTypeList = ManualFeeType.GetList();
        model.StatusList = ManualFeeReasonStatus.GetList();

        model.ManualFeeReason = manualFeeReason;

        return View("ManualFeeReasonDetail", model);
    }

    [HttpPost]
    [Route("/admin/manual/fee/reason/save")]
    [Transactional]
    public IActionResult Save(ManualFeeReasonDetailModel model)
    {
        if (!ModelState.IsValid)
        {
            throw new ValidationException(ValidationUtil.GetErrorMessages(ModelState));
        }

        ManualFeeReasonInfo manualFeeReasonFromUi = model.ManualFeeReason;
        int manualFeeReasonId = manualFeeReasonFromUi.Id;

        bool isCreate = manualFeeReasonId == 0;

        ManualFeeReasonInfo manualFeeReason;

        if (isCreate)
        {
            manualFeeReason = new ManualFeeReasonInfo();
        }
        else
        {
            manualFeeReason = session.Get<ManualFeeReasonInfo>(manualFeeReasonId);
        }

        manualFeeReason.FeeType = manualFeeReasonFromUi.FeeType;
        manualFeeReason.Content = manualFeeReasonFromUi.Content;
        manualFeeReason.Status = manualFeeReasonFromUi.Status;

        if (isCreate)
        {
            session.Save(manualFeeReason);
        }
        else
        {
            session.Update(manualFeeReason);
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "保存成功！" }
        });
    }

    [HttpPost]
    [Route("/admin/manual/fee/reason/delete")]
    [Transactional]
    public IActionResult Delete(int[] idList)
    {
        IList<ManualFeeReasonInfo> feeReasonList = session.Query<ManualFeeReasonInfo>().Where(e => idList.Contains(e.Id)).ToList();
        foreach (ManualFeeReasonInfo feeReason in feeReasonList)
        {
            /*
             todo 后面补上
            if (feeReason.FeeType == ManualFeeType.WriteOffFee)
            {
                int count = session.Query<LogisticsOrderPayInfo>().Where(e => e.WriteOffReasonId == feeReason.Id).Count();
                if (count != 0)
                {
                    throw new ValidationException($"{feeReason.FeeType?.Name} {feeReason.Content} 当前被引用中, 不能删除, 可以停用!");
                }
            }
            else
            {
                int count = session.Query<LogisticsOrderFeeInfo>().Where(e => e.ManualReasonId == feeReason.Id).Count();
                if (count != 0)
                {
                    throw new ValidationException($"{feeReason.FeeType?.Name} {feeReason.Content} 当前被引用中, 不能删除, 可以停用!");
                }
            }
            */
        }

        foreach (int id in idList)
        {
            ManualFeeReasonInfo manualFeeReason = session.Load<ManualFeeReasonInfo>(id);
            session.Delete(manualFeeReason);
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "删除成功！" },
        });
    }
}