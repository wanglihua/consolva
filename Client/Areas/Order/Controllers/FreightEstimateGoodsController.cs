using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Order.Models.FreightEstimateGoods;
using shunda.Client.Services;
using shunda.Core;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Logistics;
using shunda.Domain.LogisticsOrder;
using shunda.Service.LogisticsOrder;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Order.Controllers;

[OrderArea]
public class FreightEstimateGoodsController : Controller {
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;
    private readonly FreightEstimateSelectService freightEstimateSelectService;

    public FreightEstimateGoodsController(ISession session,
        LoginCustomerService loginCustomerService,
        FreightEstimateSelectService freightEstimateSelectService
    ) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
        this.freightEstimateSelectService = freightEstimateSelectService;
    }

    [HttpPost]
    [Route("/client/freight/estimate/goods/detail")]
    public IActionResult Detail() {
        int loginCustomerId = loginCustomerService.GetLoginCustomerId();
        var estimateSelect = session.Get<FreightEstimateSelectInfo>(loginCustomerId);

        int goodsTypeId = estimateSelect?.GoodsType.Id ?? LogisticsGoodsType.GetCustomerList().First().Id;
        string goodsValue = estimateSelect?.GoodsValue.ToString("#0.#") ?? string.Empty;
        var goodsList = session.Query<FreightEstimateSelectGoodsInfo>()
            .Where(e => e.CustomerId == loginCustomerId)
            .OrderBy(e => e.Id)
            .ToList();

        var goodsItemList = new List<FreightEstimateGoodsListItem>();
        if (goodsList.Count == 0) {
            goodsItemList.Add(new FreightEstimateGoodsListItem {
                Width = string.Empty,
                Length = string.Empty,
                Height = string.Empty,
                Weight = string.Empty,
                PacakgeCount = string.Empty,
            });
        } else {
            goodsItemList = goodsList.Select(e => new FreightEstimateGoodsListItem {
                    Width = e.Width.ToString("#0.#"),
                    Length = e.Length.ToString("#0.#"),
                    Height = e.Height.ToString("#0.#"),
                    Weight = e.Weight.ToString("#0.#"),
                    PacakgeCount = e.PackageCount.ToString(),
                })
                .ToList();
        }

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new FreightEstimateGoodsDetailModel {
                GoodsTypeList = LogisticsGoodsType.GetCustomerList(),
                GoodsTypeId = goodsTypeId,
                GoodsValue = goodsValue,
                GoodsList = goodsItemList,
            },
        });
    }

    [HttpPost]
    [Route("/client/freight/estimate/goods/save")]
    public IActionResult Save([FromBody] FreightEstimateGoodsSaveModel model) {
        if (ModelState.IsValid == false) {
            throw new ValidationException(ValidationUtil.GetErrorMessagesForMobile(ModelState));
        }

        if (model.GoodsTypeId <= 0) {
            throw new ValidationException("物品类型不能为空！");
        }

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }
}