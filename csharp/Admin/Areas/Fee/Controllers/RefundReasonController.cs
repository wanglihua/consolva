using Domain;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Domain.LogisticsOrder;
using shunda.Core;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Pass.PassRes;
using Web.Areas.Admin.Models.RefundReason;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Fee.Controllers;

[FeeArea]
[AdminAuth(PassRes.Fee.FeeRefundReason)]
public class RefundReasonController : Controller
{
    private ISession session = null;

    public RefundReasonController(ISession session)
    {
        this.session = session;
    }

    [HttpGet]
    [Route("/admin/fee/refund/reason")]
    [NoHibernateAutoTransaction]
    public IActionResult Index()
    {
        RefundReasonIndexModel model = new RefundReasonIndexModel();
        return View("RefundReasonIndex", model);
    }

    [HttpPost]
    [Route("/admin/fee/refund/reason/list")]
    [NoHibernateAutoTransaction]
    public IActionResult List(RefundReasonListQueryParam queryParam)
    {
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

        foreach (FeeRefundReasonInfo refundReasonInfo in refundReasonList)
        {
            // refundReasonInfo
        }

        RefundReasonListModel model = new RefundReasonListModel(listPager);
        model.RefundReasonList = refundReasonList;

        return View("RefundReasonList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(RefundReasonListQueryParam queryParam)
    {
        ICriteria criteria = session.CreateCriteria<FeeRefundReasonInfo>();
        criteria.SetReadOnly(true);

        return criteria;
    }

    [HttpPost]
    [Route("/admin/fee/refund/reason/detail/{id:int?}")]
    [NoHibernateAutoTransaction]
    public IActionResult Detail(int? id)
    {
        session.DefaultReadOnly = true;

        FeeRefundReasonInfo refundReason = null;

        if (id == null || id.Value == 0)
        {
            refundReason = new FeeRefundReasonInfo();
        }
        else
        {
            refundReason = session.Get<FeeRefundReasonInfo>(id.Value);
            if (refundReason == null)
            {
                throw new ValidationException("指定的数据项不存在！");
            }
        }

        RefundReasonDetailModel model = new RefundReasonDetailModel();
        model.RefundReason = refundReason;

        model.StatusList = FeeRefundReasonStatus.GetList();

        return View("RefundReasonDetail", model);
    }

    [HttpPost]
    [Route("/admin/fee/refund/reason/save")]
    public IActionResult Save(RefundReasonDetailModel model)
    {
        if (!ModelState.IsValid)
        {
            throw new ValidationException(ValidationUtil.GetErrorMessages(ModelState));
        }

        FeeRefundReasonInfo refundReasonFromUi = model.RefundReason;
        int refundReasonId = refundReasonFromUi.Id;

        bool isCreate = refundReasonId == 0;

        FeeRefundReasonInfo refundReason = null;

        if (isCreate)
        {
            refundReason = new FeeRefundReasonInfo();
        }
        else
        {
            refundReason = session.Get<FeeRefundReasonInfo>(refundReasonId);
        }

        refundReason.Content = refundReasonFromUi.Content;
        refundReason.Status = refundReasonFromUi.Status;

        if (isCreate)
        {
            session.Save(refundReason);
        }
        else
        {
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
    public IActionResult Delete(int[] idList)
    {
        foreach (int id in idList)
        {
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