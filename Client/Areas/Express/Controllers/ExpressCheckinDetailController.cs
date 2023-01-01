using Microsoft.AspNetCore.Mvc;
using NHibernate.Linq;
using shunda.Client.Areas.Express.Models.ExpressCheckin;
using shunda.Client.Areas.Express.Models.ExpressCheckinDetail;
using shunda.Client.Services;
using shunda.Core;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Express;
using shunda.Domain.Logistics;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Express.Controllers;

[ExpressArea]
public class ExpressCheckinDetailController : Controller {
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;

    public ExpressCheckinDetailController(ISession session,
        LoginCustomerService loginCustomerService
    ) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
    }

    [HttpPost]
    [Route("/client/express/checkin/detail")]
    public IActionResult Detail([FromBody] IdParam param) {
        if (ModelState.IsValid == false) {
            throw new ValidationException(ValidationUtil.GetErrorMessagesForMobile(ModelState));
        }

        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        int expressId = param.Id;
        var express = session.Get<ExpressInfo>(expressId);
        if (express == null) {
            throw new ValidationException($"指定的快递单号#{expressId}不存在！");
        }

        if ((express.Customer?.Id ?? 0) != loginCustomerId) {
            throw new ValidationException($"指定的快递单号不属于当前登录会员#{loginCustomerId}！");
        }

        // 入库备注
        var expressWarehouseRemark = session.Query<ExpressRemarkInfo>()
            .Where(e => e.Express!.Id == expressId
                        && e.Type == ExpressRemarkType.Warehouse)
            .Take(1)
            .SingleOrDefault();
        var warehouseRemark = expressWarehouseRemark?.Remark ?? string.Empty;

        // 物品列表
        var goodsList = session.Query<ExpressGoodsInfo>()
            .Fetch(e => e.Goods)
            .Where(e => e.Express.Id == expressId)
            .ToList();
        var goodsCount = goodsList.Count;

        var goodsItemList = goodsList.Select(e => new ExpressCheckinGoodsListItem {
                Id = e.Id,
                GoodsName = e.Goods.Name,
                Quantity = e.Quantity,
                Unit = e.Unit,
                Price = e.Price.ToString("#0.00"),
            })
            .ToList();

        ExpressCheckinDetailModel model = new ExpressCheckinDetailModel {
            Id = express.Id,
            TrackingNo = express.TrackingNo,
            ExpressCompany = express.ExpressCompany,
            GoodsType = express.GoodsType.Name,
            Forbidden = express.GoodsType == LogisticsGoodsType.Forbidden,
            CheckinTrackingNo = express.CheckinTrackingNo,
            AddDate = express.AddTime.ToString("yyyy-MM-dd"),
            ClaimDate = express.ClaimTime?.ToString("yyyy-MM-dd") ?? string.Empty,
            Status = express.Status.Name,
            GoodsCount = goodsCount,
            WaitOrder = express.Status == ExpressStatus.WaitOrder,
            WarehouseRemark = warehouseRemark,

            GoodsList = goodsItemList,
        };

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = model,
        });
    }
}