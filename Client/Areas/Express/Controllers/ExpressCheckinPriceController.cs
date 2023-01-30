using Microsoft.AspNetCore.Mvc;
using NHibernate.Linq;
using shunda.Client.Areas.Express.Models.ExpressCheckin;
using shunda.Client.Areas.Express.Models.ExpressCheckinPrice;
using shunda.Client.Services;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Express;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Express.Controllers;

[ExpressArea]
public class ExpressCheckinPriceController : Controller {
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;

    public ExpressCheckinPriceController(ISession session,
        LoginCustomerService loginCustomerService
    ) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
    }

    [HttpPost]
    [Route("/client/express/checkin/price")]
    public IActionResult Price([FromBody] IdParam param) {
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
            throw new ValidationException($"指定的快递单号{express.TrackingNo}不属于当前登录会员#{loginCustomerId}！");
        }

        var goodsList = session.Query<ExpressGoodsInfo>()
            .Fetch(e => e.Goods)
            .Where(e => e.Express.Id == expressId)
            .ToList();

        var model = new ExpressCheckinPriceModel {
            Id = express.Id,
            TrackingNo = express.TrackingNo,
            CheckinTrackingNo = express.CheckinTrackingNo,
            GoodsList = goodsList.OrderBy(e => e.Id)
                .Select(e => new ExpressCheckinGoodsListItem {
                    Id = e.Id,
                    GoodsName = e.Goods.Name,
                    Quantity = e.Quantity,
                    Unit = e.Unit,
                    Price = e.Price.ToString("#0.00"),
                })
                .ToList(),
        };

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = model,
        });
    }

    [HttpPost]
    [Route("/client/express/checkin/price/save")]
    [Transactional]
    public IActionResult PriceSave([FromBody] ExpressCheckinPriceSaveParam param) {
        if (ModelState.IsValid == false) {
            throw new ValidationException(ValidationUtil.GetErrorMessagesForMobile(ModelState));
        }

        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        var expressId = param.ExpressId;
        var express = session.Get<ExpressInfo>(expressId);
        if (express == null) {
            throw new ValidationException($"指定的快递单号#{expressId}不存在！");
        }

        if ((express.Customer?.Id ?? 0) != loginCustomerId) {
            throw new ValidationException($"指定的快递单号{express.TrackingNo}不属于当前登录会员#{loginCustomerId}！");
        }

        if (param.GoodsPriceList == null || param.GoodsPriceList.Count == 0) {
            throw new ValidationException("物品价格列表不能为空！");
        }

        var expressGoodsIdList = param.GoodsPriceList.Select(e => e.Id).ToList();
        var expressGoodsList = session.Query<ExpressGoodsInfo>()
            .Fetch(e => e.Goods)
            .Where(e => expressGoodsIdList.Contains(e.Id))
            .ToList();

        var goodsPriceList = param.GoodsPriceList;

        // 检查物品
        foreach (var goodsPrice in goodsPriceList) {
            int expressGoodsId = goodsPrice.Id;
            var expressGoods = expressGoodsList.Where(e => e.Id == expressGoodsId).Take(1).SingleOrDefault();
            if (expressGoods == null) {
                throw new ValidationException($"指定的物品#{expressGoodsId}不存在！");
            }
        }

        // 检查价格录入
        foreach (var goodsPrice in goodsPriceList) {
            if (decimal.TryParse(goodsPrice.Price, out decimal price) == false || price < 0) {
                int expressGoodsId = goodsPrice.Id;
                var expressGoods = expressGoodsList.Where(e => e.Id == expressGoodsId).Take(1).SingleOrDefault();
                // 由于前面的判断，肯定不为 null
                if (expressGoods != null) {
                    throw new ValidationException($"物品 {expressGoods.Goods.Name} 价格录入有误！");
                }
            }
        }

        foreach (var goodsPrice in goodsPriceList) {
            // 由于前面判断，肯定成立
            if (decimal.TryParse(goodsPrice.Price, out decimal price) == true && price > 0) {
                int expressGoodsId = goodsPrice.Id;
                var expressGoods = expressGoodsList.Where(e => e.Id == expressGoodsId).Take(1).SingleOrDefault();
                // 由于前面的判断，肯定不为 null
                if (expressGoods != null) {
                    expressGoods.Price = price;
                    session.Update(expressGoods);
                }
            }
        }

        session.Flush();

        return Json(new JsonResultData {
            Success = true,
            Message = "价格保存成功！",
        });
    }
}