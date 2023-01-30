using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using shunda.Admin.Areas.Fee.Models.FeeRefundReason;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Fee;
using shunda.Domain.Pass.PassRes;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Fee.Controllers;

[FeeArea]
[AdminAuth(PassRes.Fee.FeeRefundReason)]
public class FeeRefundReasonController : Controller {
    private readonly ISession session;

    public FeeRefundReasonController(ISession session) {
        this.session = session;
    }

    [HttpGet]
    [Route("/admin/fee/refund/reason")]
    public IActionResult Index() {
        FeeRefundReasonIndexModel model = new FeeRefundReasonIndexModel();
        return View("FeeRefundReasonIndex", model);
    }

    [HttpPost]
    [Route("/admin/fee/refund/reason/list")]
    public IActionResult List(FeeRefundReasonListQueryParam queryParam) {
        int totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<FeeRefundReasonInfo> refundReasonList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<FeeRefundReasonInfo>();

        foreach (FeeRefundReasonInfo refundReasonInfo in refundReasonList) {
            // refundReasonInfo
        }

        FeeRefundReasonListModel model = new FeeRefundReasonListModel(listPager);
        model.RefundReasonList = refundReasonList;

        return View("FeeRefundReasonList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(FeeRefundReasonListQueryParam queryParam) {
        ICriteria criteria = session.CreateCriteria<FeeRefundReasonInfo>();
        criteria.SetReadOnly(true);

        return criteria;
    }

    [HttpPost]
    [Route("/admin/fee/refund/reason/detail/{id:int?}")]
    public IActionResult Detail(int? id) {
        session.DefaultReadOnly = true;

        FeeRefundReasonInfo? refundReason;

        if (id == null || id.Value == 0) {
            refundReason = new FeeRefundReasonInfo();
        } else {
            refundReason = session.Get<FeeRefundReasonInfo>(id.Value);
            if (refundReason == null) {
                throw new ValidationException("指定的数据项不存在！");
            }
        }

        FeeRefundReasonDetailModel model = new FeeRefundReasonDetailModel();
        model.RefundReason = refundReason;

        model.StatusList = FeeRefundReasonStatus.GetList();

        return View("FeeRefundReasonDetail", model);
    }

    [HttpPost]
    [Route("/admin/fee/refund/reason/save")]
    [Transactional]
    public IActionResult Save(FeeRefundReasonSaveModel model) {
        if (!ModelState.IsValid) {
            throw new ValidationException(ValidationUtil.GetErrorMessages(ModelState));
        }

        FeeRefundReasonInfo refundReasonFromUi = model.RefundReason;
        int refundReasonId = refundReasonFromUi.Id;

        bool isCreate = refundReasonId == 0;

        FeeRefundReasonInfo? refundReason;

        if (isCreate) {
            refundReason = new FeeRefundReasonInfo();
        } else {
            refundReason = session.Get<FeeRefundReasonInfo>(refundReasonId);
        }

        refundReason.Content = refundReasonFromUi.Content;
        refundReason.Status = refundReasonFromUi.Status;

        if (isCreate) {
            session.Save(refundReason);
        } else {
            session.Update(refundReason);
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "保存成功！" }
        });
    }

    [HttpPost]
    [Route("/admin/fee/refund/reason/delete")]
    [Transactional]
    public IActionResult Delete(int[] idList) {
        foreach (int id in idList) {
            FeeRefundReasonInfo refundReason = session.Load<FeeRefundReasonInfo>(id);
            session.Delete(refundReason);
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "删除成功！" },
        });
    }
}