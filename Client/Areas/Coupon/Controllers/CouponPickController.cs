using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Coupon.Models.CouponPick;
using shunda.Client.Services;
using shunda.Core.Hibernate;
using shunda.Core.Web;
using shunda.Domain.Coupon;
using shunda.Service.Coupon;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Coupon.Controllers;

public class CouponPickController : Controller {
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;
    private readonly CouponBatchService couponBatchService;

    public CouponPickController(ISession session,
        LoginCustomerService loginCustomerService,
        CouponBatchService couponBatchService
    ) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
        this.couponBatchService = couponBatchService;
    }

    [HttpPost]
    [Route("/client/coupon/pick/list")]
    public IActionResult List([FromBody] CouponPickListParam param) {
        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        var model = GetListData(param, loginCustomerId);
        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = model,
        });
    }

    [NonAction]
    private CouponPickListModel GetListData(CouponPickListParam param, int customerId) {
        DateTime today = DateTime.Today;
        var batchList = session.Query<CouponBatchInfo>()
            .Where(e => e.GetMethod == CouponGetMethod.Pick
                        && e.Status == CouponBatchStatus.Release
                        && e.Expire >= today)
            .OrderByDescending(e => e.Id)
            .ToList();

        // 获取这些批次中，登录客户领取的优惠券
        var batchIdList = batchList.Select(e => e.Id).ToList();
        var couponPickList = session.Query<CouponInfo>()
            .Where(e => e.Customer.Id == customerId
                        && e.GetMethod == CouponGetMethod.Pick
                        && batchIdList.Contains(e.Batch.Id)
            )
            .ToList();

        // 各个批次已领取的数量
        var batchPickCountDict = new Dictionary<int, int>();
        foreach (var coupon in couponPickList) {
            int batchId = coupon.Batch.Id;
            if (batchPickCountDict.TryGetValue(batchId, out int pickCount) == false) {
                batchPickCountDict[batchId] = 1;
            } else {
                batchPickCountDict[batchId] = pickCount + 1;
            }
        }

        var batchItemList = new List<CouponPickListItem>();
        foreach (var batch in batchList) {
            int batchId = batch.Id;
            if (batchPickCountDict.TryGetValue(batchId, out int pickCount) == false // 批次中客户还没领取
                || pickCount < batch.MaxQtyPerCustomer // 批次中客户还没领完
               ) {
                batchItemList.Add(new CouponPickListItem {
                    Id = batch.Id,
                    Name = batch.Name,
                    Type = batch.Type.Name,
                    Rule = batch.Rule,
                    RuleDesc = batch.RuleDesc,
                    Expire = batch.Expire.ToString("yyyy-MM-dd"),
                });
            }
        }

        return new CouponPickListModel {
            BatchList = batchItemList,
            BatchCount = batchItemList.Count,
        };
    }

    [HttpPost]
    [Route("/client/coupon/pick/save")]
    [Transactional]
    public IActionResult Save([FromBody] CouponPickSaveParam param) {
        int batchId = param.BatchId;
        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        couponBatchService.Pick(batchId: batchId, customerId: loginCustomerId);

        var listModel = GetListData(param, loginCustomerId);

        return Json(new JsonResultData {
            Success = true,
            Message = "优惠券领取成功！",
            Data = listModel,
        });
    }
}