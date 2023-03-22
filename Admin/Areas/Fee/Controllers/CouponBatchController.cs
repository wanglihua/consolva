using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using shunda.Admin.Areas.Fee.Models.CouponBatch;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Coupon;
using shunda.Domain.Pass.PassRes;
using shunda.Service.Coupon;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Fee.Controllers;

[FeeArea]
[AdminAuth(PassRes.Fee.Coupon)]
public class CouponBatchController : Controller {
    private readonly ISession session;
    private readonly CouponBatchService couponBatchService;

    public CouponBatchController(
        ISession session,
        CouponBatchService couponBatchService) {
        this.session = session;
        this.couponBatchService = couponBatchService;
    }

    [HttpGet]
    [Route("/admin/coupon/batch")]
    [Transactional] // 需要事务
    public IActionResult Index() {
        CouponBatchIndexModel model = new CouponBatchIndexModel();

        model.TypeList = CouponType.GetList();
        model.StatusList = CouponBatchStatus.GetList();

        // 在这个时机检查和设置已过期状态
        couponBatchService.CheckAndSetExpirePickBatch();

        return View("CouponBatchIndex", model);
    }

    [HttpPost]
    [Route("/admin/coupon/batch/list")]
    public IActionResult List(CouponBatchListQueryParam queryParam) {
        int totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<CouponBatchInfo> couponBatchList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<CouponBatchInfo>();

        foreach (CouponBatchInfo couponBatch in couponBatchList) {
        }

        CouponBatchListModel model = new CouponBatchListModel(listPager);
        model.CouponBatchList = couponBatchList;

        return View("CouponBatchList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(CouponBatchListQueryParam queryParam) {
        ICriteria criteria = session.CreateCriteria<CouponBatchInfo>();
        criteria = criteria.SetReadOnly(true);

        criteria = criteria.Add(Restrictions.Or(
            Restrictions.Where<CouponBatchInfo>(e => e.GetMethod == CouponGetMethod.Pick),
            Restrictions.Where<CouponBatchInfo>(e => e.GetMethod == CouponGetMethod.Grant))
        );

        if (string.IsNullOrWhiteSpace(queryParam.NameQuery) == false) {
            string nameQuery = queryParam.NameQuery.Trim();
            criteria = criteria.Add(Restrictions.On<CouponBatchInfo>(e => e.Name).IsInsensitiveLike(nameQuery, MatchMode.Anywhere));
        }

        if (queryParam.TypeQuery != null && queryParam.TypeQuery != CouponType.None) {
            var typeQuery = queryParam.TypeQuery;
            criteria = criteria.Add(Restrictions.Where<CouponBatchInfo>(e => e.Type == typeQuery));
        }

        if (queryParam.StatusQuery != null && queryParam.StatusQuery != CouponBatchStatus.None) {
            var statusQuery = queryParam.StatusQuery;
            criteria = criteria.Add(Restrictions.Where<CouponBatchInfo>(e => e.Status == statusQuery));
        }

        if (queryParam.ExpireBeginQuery != null) {
            var expireBeginQuery = queryParam.ExpireBeginQuery.Value.Date;
            criteria = criteria.Add(Restrictions.Where<CouponBatchInfo>(e => e.Expire >= expireBeginQuery));
        }

        if (queryParam.ExpireEndQuery != null) {
            var expireEndQuery = queryParam.ExpireEndQuery.Value.Date.AddDays(1);
            criteria = criteria.Add(Restrictions.Where<CouponBatchInfo>(e => e.Expire < expireEndQuery));
        }

        return criteria;
    }

    [HttpPost]
    [Route("/admin/coupon/batch/detail")]
    public IActionResult Detail(int id) {
        session.DefaultReadOnly = true;

        CouponBatchInfo? couponBatch;

        CouponBatchDetailModel model = new CouponBatchDetailModel();

        if (id == 0) {
            couponBatch = new CouponBatchInfo();
            model.Status = CouponBatchStatus.New;
            model.Expire = DateTime.Today;
        } else {
            couponBatch = session.Get<CouponBatchInfo>(id);
            if (couponBatch == null) {
                throw new ValidationException($"指定的 优惠券批次#{id} 不存在！");
            }

            model.Status = couponBatch.Status;
            model.Expire = couponBatch.Expire;
            model.Rule = couponBatch.Rule;
            model.RuleDesc = couponBatch.RuleDesc;
        }

        model.Released = model.Status.Released();

        model.CouponBatchId = couponBatch.Id;

        model.Name = couponBatch.Name;
        model.Type = couponBatch.Type;
        model.TotalQuantity = couponBatch.TotalQuantity;
        model.MaxQtyPerCustomer = couponBatch.MaxQtyPerCustomer;

        model.TypeList = CouponType.GetList();
        model.StatusList = CouponBatchStatus.GetList();

        return View("CouponBatchDetail", model);
    }

    [HttpPost]
    [Route("/admin/coupon/batch/save")]
    [Transactional]
    public IActionResult Save(CouponBatchDetailModel model) {
        if (!ModelState.IsValid) {
            throw new ValidationException(ValidationUtil.GetErrorMessages(this.ModelState));
        }

        int couponBatchId = model.CouponBatchId;

        bool isCreate = couponBatchId == 0;

        CouponBatchInfo? couponBatch;

        if (isCreate) {
            couponBatch = new CouponBatchInfo();
            couponBatch.Status = CouponBatchStatus.New;
        } else {
            couponBatch = session.Get<CouponBatchInfo>(couponBatchId);
            if (couponBatch == null) {
                throw new ValidationException($"指定的 优惠券批次#{couponBatchId} 不存在！");
            }

            // 已经投放的，不能再编辑
            if (couponBatch.Status.Released()) {
                throw new ValidationException($"指定的 优惠券批次#{couponBatchId} 已投放，不能再修改！");
            }
        }

        couponBatch.GetMethod = CouponGetMethod.Pick;
        couponBatch.Name = model.Name;
        couponBatch.Type = model.Type;
        couponBatch.Expire = model.Expire;
        couponBatch.TotalQuantity = model.TotalQuantity;
        // couponBatch.RuleDesc = model.RuleDesc; 这个这里不能编辑
        couponBatch.MaxQtyPerCustomer = model.MaxQtyPerCustomer;
        // couponBatch.Status = model.Status; 状态不能通过详细编辑改变

        if (isCreate) {
            session.Save(couponBatch);
        } else {
            session.Update(couponBatch);
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "保存成功！" }
        });
    }

    [HttpPost]
    [Route("/admin/coupon/batch/delete")]
    [Transactional]
    public IActionResult Delete(int[] idList) {
        if (idList == null || idList.Length == 0) {
            throw new ValidationException("请选择要删除的数据！");
        }

        var batchList = session.Query<CouponBatchInfo>()
            .Where(e => idList.Contains(e.Id))
            .ToList();

        foreach (var batch in batchList) {
            if (batch.Status.Released()) {
                throw new ValidationException($"优惠券批次#{batch.Name}已投放不能删除！");
            }
        }

        foreach (var batch in batchList) {
            session.Delete(batch);
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "删除成功！" },
        });
    }

    [HttpPost]
    [Route("/admin/coupon/batch/release")]
    [Transactional]
    public IActionResult Release(int batchId) {
        var batch = session.Get<CouponBatchInfo>(batchId);
        if (batch == null) {
            throw new ValidationException($"指定的优惠券批次#{batchId}不存在！");
        }

        // 检查规则是否已经编辑并且合法
        couponBatchService.CheckRuleValid(batchId);

        if ((batch.Status?.Released() ?? false) == true) {
            throw new ValidationException($"优惠券批次#{batch.Name} 已经是投放客户或后续状态！");
        }

        batch.Status = CouponBatchStatus.Release;

        session.Update(batch);
        session.Flush();

        // 更新批次已领取总数量，实际这里是检查后台发放的数据是否已达到总数量
        couponBatchService.SumAndSaveBatchTotalPicked(batchId);

        if (batch.Status == CouponBatchStatus.Release) // 发布状态才设置
        {
            if (batch.TotalQuantity <= batch.PickQuantity) {
                batch.Status = CouponBatchStatus.Picked;
                session.Update(batch);

                session.Flush();
            }
        }

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", $"优惠券批次#{batch.Name} 投放客户成功！" },
        });
    }

    [HttpPost]
    [Route("/admin/coupon/batch/show/rule")]
    public IActionResult ShowRule(int batchId) {
        var batch = session.Get<CouponBatchInfo>(batchId);
        if (batch == null) {
            throw new ValidationException($"指定的优惠券批次#{batchId}不存在！");
        }

        if (batch.Type == CouponType.Deduct) {
            var rule = session.Query<CouponRuleDeductInfo>()
                .Where(e => e.Batch.Id == batchId)
                .Take(1)
                .SingleOrDefault();

            int ruleId = rule?.Id ?? 0;
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            return RedirectToAction("Detail", "CouponRuleDeduct", new { id = ruleId, batchId = batchId, ts = timestamp });
        }

        if (batch.Type == CouponType.Discount) {
            var rule = session.Query<CouponRuleDiscountInfo>()
                .Where(e => e.Batch.Id == batchId)
                .Take(1)
                .SingleOrDefault();

            int ruleId = rule?.Id ?? 0;
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            return RedirectToAction("Detail", "CouponRuleDiscount", new { id = ruleId, batchId = batchId, ts = timestamp });
        }

        if (batch.Type == CouponType.PriceBreak) {
            var rule = session.Query<CouponRulePriceBreakInfo>()
                .Where(e => e.Batch.Id == batchId)
                .Take(1)
                .SingleOrDefault();

            int ruleId = rule?.Id ?? 0;
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            return RedirectToAction("Detail", "CouponRulePriceBreak", new { id = ruleId, batchId = batchId, ts = timestamp });
        }

        throw new ValidationException($"指定的优惠券批次#{batchId}类型{batch.Type?.Name}不合法！！");
    }
}