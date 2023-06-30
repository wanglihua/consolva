using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using shunda.Admin.Areas.Fee.Models.CouponBatchFollow;
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
public class CouponBatchFollowController : Controller {
    private readonly ISession session;
    private readonly CouponBatchService couponBatchService;

    public CouponBatchFollowController(
        ISession session,
        CouponBatchService couponBatchService) {
        this.session = session;
        this.couponBatchService = couponBatchService;
    }

    [HttpGet]
    [Route("/admin/coupon/batch/follow")]
    [Transactional] // 需要事务
    public IActionResult Index() {
        CouponBatchFollowIndexModel model = new CouponBatchFollowIndexModel();

        model.TypeList = CouponType.GetList();
        model.StatusList = CouponBatchStatus.GetList();

        // 在这个时机检查和设置已过期状态
        couponBatchService.CheckAndSetExpireFollowBatch();

        return View("CouponBatchFollowIndex", model);
    }

    [HttpPost]
    [Route("/admin/coupon/batch/follow/list")]
    public IActionResult List(CouponBatchFollowListQueryParam queryParam) {
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

        CouponBatchFollowListModel model = new CouponBatchFollowListModel(listPager);
        model.CouponBatchList = couponBatchList;

        return View("CouponBatchFollowList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(CouponBatchFollowListQueryParam queryParam) {
        ICriteria criteria = session.CreateCriteria<CouponBatchInfo>();
        criteria = criteria.SetReadOnly(true);

        criteria = criteria.Add(Restrictions.Where<CouponBatchInfo>(e => e.GetMethod == CouponGetMethod.Follow));

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
    [Route("/admin/coupon/batch/follow/detail")]
    public IActionResult Detail(int id) {
        session.DefaultReadOnly = true;

        CouponBatchInfo? couponBatch;

        CouponBatchFollowDetailModel model = new CouponBatchFollowDetailModel();

        if (id == 0) {
            couponBatch = new CouponBatchInfo();
            model.Status = CouponBatchStatus.New;
        } else {
            couponBatch = session.Get<CouponBatchInfo>(id);
            if (couponBatch == null) {
                throw new ValidationException($"指定的 优惠券批次#{id} 不存在！");
            }

            model.Status = couponBatch.Status;
            model.Rule = couponBatch.Rule;
            model.RuleDesc = couponBatch.RuleDesc;
        }

        model.Released = model.Status.Released();

        model.CouponBatchId = couponBatch.Id;

        model.Name = couponBatch.Name;
        model.Type = couponBatch.Type;
        model.MaxQtyPerCustomer = couponBatch.MaxQtyPerCustomer;
        model.ExpireMonth = couponBatch.ExpireMonth;

        model.TypeList = CouponType.GetList();
        model.StatusList = CouponBatchStatus.GetList();

        return View("CouponBatchFollowDetail", model);
    }

    [HttpPost]
    [Route("/admin/coupon/batch/follow/save")]
    [Transactional]
    public IActionResult Save(CouponBatchFollowDetailModel model) {
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

        if (model.ExpireMonth <= 0) {
            throw new ValidationException("过期月份数必须大于0！");
        }

        couponBatch.GetMethod = CouponGetMethod.Follow;
        couponBatch.Expire = DateTime.Now; // 必须要给你值
        couponBatch.Name = model.Name;
        couponBatch.Type = model.Type;
        couponBatch.TotalQuantity = 0;
        // couponBatch.RuleDesc = model.RuleDesc; 这个这里不能编辑
        couponBatch.MaxQtyPerCustomer = model.MaxQtyPerCustomer;
        couponBatch.ExpireMonth = model.ExpireMonth;
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

    // 跟 CouponBatchController 里面的一样
    [HttpPost]
    [Route("/admin/coupon/batch/follow/delete")]
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
                throw new ValidationException($"优惠券批次#{batch.Name}已投放过，不能删除，可以选择停用！");
            }
        }
        
        // 先删除对应的规则设置
        var batchIdList = batchList.Select(e => e.Id).ToList();
        var couponRuleDeductList = session.Query<CouponRuleDeductInfo>()
            .Where(e => batchIdList.Contains(e.Batch.Id))
            .ToList();
        foreach (var couponRuleDeduct in couponRuleDeductList) {
            session.Delete(couponRuleDeduct);
        }

        var couponRuleDiscountList = session.Query<CouponRuleDiscountInfo>()
            .Where(e => batchIdList.Contains(e.Batch.Id))
            .ToList();
        foreach (var couponRuleDiscount in couponRuleDiscountList) {
            session.Delete(couponRuleDiscount);
        }

        var couponRulePriceBreakList = session.Query<CouponRulePriceBreakInfo>()
            .Where(e => batchIdList.Contains(e.Batch.Id))
            .ToList();
        foreach (var couponRulePriceBreak in couponRulePriceBreakList) {
            session.Delete(couponRulePriceBreak);
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
    [Route("/admin/coupon/batch/follow/release")]
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
        // couponBatchService.SumAndSaveBatchTotalPicked(batchId); 这里不用设置这个

        // 发布状态才设置
        // 这里不用设置这个
        // if (batch.Status == CouponBatchStatus.Release) {
        //     if (batch.TotalQuantity <= batch.PickQuantity) {
        //         batch.Status = CouponBatchStatus.Picked;
        //         session.Update(batch);
        //
        //         session.Flush();
        //     }
        // }

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", $"优惠券批次#{batch.Name} 投放客户成功！" },
        });
    }

    // 跟 CouponBatchController 里面的一样
    [HttpPost]
    [Route("/admin/coupon/batch/follow/show/rule")]
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