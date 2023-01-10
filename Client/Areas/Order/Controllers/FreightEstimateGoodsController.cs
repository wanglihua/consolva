using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Order.Models.FreightEstimateGoods;
using shunda.Client.Services;
using shunda.Core;
using shunda.Core.Hibernate;
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
            // 保证总是有一个箱型
            goodsItemList.Add(new FreightEstimateGoodsListItem {
                Id = 0,
                Width = string.Empty,
                Length = string.Empty,
                Height = string.Empty,
                Weight = string.Empty,
                PackageCount = string.Empty,
            });
        } else {
            goodsItemList = goodsList.Select(e => new FreightEstimateGoodsListItem {
                    Id = e.Id,
                    Width = e.Width.ToString("#0.#"),
                    Length = e.Length.ToString("#0.#"),
                    Height = e.Height.ToString("#0.#"),
                    Weight = e.Weight.ToString("#0.#"),
                    PackageCount = e.PackageCount.ToString(),
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
    [Transactional]
    public IActionResult Save([FromBody] FreightEstimateGoodsSaveModel model) {
        if (ModelState.IsValid == false) {
            throw new ValidationException(ValidationUtil.GetErrorMessagesForMobile(ModelState));
        }

        if (model.GoodsTypeId <= 0) {
            throw new ValidationException("物品类型不能为空！");
        }

        var goodsType = LogisticsGoodsType.GetById(model.GoodsTypeId);
        if (goodsType == LogisticsGoodsType.None || goodsType == LogisticsGoodsType.Forbidden) {
            throw new ValidationException("物品类型不存在或无效！");
        }

        if (string.IsNullOrWhiteSpace(model.GoodsValue)) {
            throw new ValidationException("总货值不能为空！");
        }

        if (decimal.TryParse(model.GoodsValue.Trim(), out decimal goodsValue) == false || goodsValue <= 0) {
            throw new ValidationException("总货值录入不合法！");
        }

        if (model.GoodsList == null || model.GoodsList.Count == 0) {
            throw new ValidationException("物品箱型列表不能为空！");
        }

        int index = 0;
        foreach (var goodsItem in model.GoodsList) {
            // 长度
            if (string.IsNullOrWhiteSpace(goodsItem.Length)) {
                throw new ValidationException($"物品箱型{index + 1}长度不能为空！");
            }

            if (decimal.TryParse(goodsItem.Length.Trim(), out decimal length) == false || length <= 0) {
                throw new ValidationException($"物品箱型{index + 1}长度录入有误！");
            }

            // 宽度
            if (string.IsNullOrWhiteSpace(goodsItem.Width)) {
                throw new ValidationException($"物品箱型{index + 1}宽度不能为空！");
            }

            if (decimal.TryParse(goodsItem.Width.Trim(), out decimal width) == false || width <= 0) {
                throw new ValidationException($"物品箱型{index + 1}宽度录入有误！");
            }

            // 高度
            if (string.IsNullOrWhiteSpace(goodsItem.Height)) {
                throw new ValidationException($"物品箱型{index + 1}高度不能为空！");
            }

            if (decimal.TryParse(goodsItem.Height.Trim(), out decimal height) == false || height <= 0) {
                throw new ValidationException($"物品箱型{index + 1}高度录入有误！");
            }

            // 重量
            if (string.IsNullOrWhiteSpace(goodsItem.Weight)) {
                throw new ValidationException($"物品箱型{index + 1}重量不能为空！");
            }

            if (decimal.TryParse(goodsItem.Weight.Trim(), out decimal weight) == false || weight <= 0) {
                throw new ValidationException($"物品箱型{index + 1}重量录入有误！");
            }

            // 箱数
            if (string.IsNullOrWhiteSpace(goodsItem.PackageCount)) {
                throw new ValidationException($"物品箱型{index + 1}箱数不能为空！");
            }

            if (int.TryParse(goodsItem.PackageCount.Trim(), out int packageCount) == false || packageCount <= 0) {
                throw new ValidationException($"物品箱型{index + 1}箱数录入有误！");
            }

            index++;
        }

        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        // 保存 物品类型 和 总货值
        var estimateSelect = session.Get<FreightEstimateSelectInfo>(loginCustomerId);
        if (estimateSelect == null) {
            estimateSelect = new FreightEstimateSelectInfo {
                CustomerId = loginCustomerId,
            };
        }

        estimateSelect.GoodsType = goodsType;
        estimateSelect.GoodsValue = goodsValue;

        session.SaveOrUpdate(estimateSelect);
        session.Flush();

        // 保存物品列表

        var goodsItemCount = model.GoodsList.Count;
        var goodsList = session.Query<FreightEstimateSelectGoodsInfo>()
            .Where(e => e.CustomerId == loginCustomerId)
            .ToList();

        // 去掉多余的数量
        var goodsListToDelete = goodsList.Skip(goodsItemCount).ToList();
        foreach (var goods in goodsListToDelete) {
            session.Delete(goods);
        }

        session.Flush();

        // 取需要保留的部分
        goodsList = goodsList.Take(goodsItemCount).ToList();

        index = 0;
        foreach (var goodsItem in model.GoodsList) {
            var goods = goodsList.Skip(index).Take(1).SingleOrDefault();
            if (goods == null) {
                goods = new FreightEstimateSelectGoodsInfo {
                    CustomerId = loginCustomerId,
                };
            }

            goods.Length = decimal.Parse(goodsItem.Length);
            goods.Width = decimal.Parse(goodsItem.Width);
            goods.Height = decimal.Parse(goodsItem.Height);
            goods.Weight = decimal.Parse(goodsItem.Weight);
            goods.PackageCount = int.Parse(goodsItem.PackageCount);

            session.SaveOrUpdate(goods);

            index++;
        }

        session.Flush();

        return Json(new JsonResultData {
            Success = true,
            Message = "物品保存成功！",
        });
    }
}