using Microsoft.AspNetCore.Mvc;
using NHibernate.Linq;
using shunda.Client.Areas.Order.Models.OrderCreateSummary;
using shunda.Client.Services;
using shunda.Core;
using shunda.Core.Web;
using shunda.Domain.Express;
using shunda.Domain.LogisticsOrder;
using shunda.Service.Logistics;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Order.Controllers;

[OrderArea]
public class OrderCreateSummaryController : Controller {
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;
    private readonly LogisticsGoodsTypeService logisticsGoodsTypeService;

    public OrderCreateSummaryController(ISession session,
        LoginCustomerService loginCustomerService,
        LogisticsGoodsTypeService logisticsGoodsTypeService
    ) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
        this.logisticsGoodsTypeService = logisticsGoodsTypeService;
    }

    [HttpPost]
    [Route("/client/order/create/summary")]
    public IActionResult Summary() {
        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        #region 快递单号

        IList<int> expressIdList = session.Query<LogisticsOrderCreateSelectExpressInfo>()
            .Where(e => e.CustomerId == loginCustomerId)
            .Select(e => e.ExpressId)
            .ToList();

        var expressList = session.Query<ExpressInfo>()
            .Where(e => expressIdList.Contains(e.Id)).OrderBy(e => e.Id)
            .ToList();

        if (expressList.Count == 0) {
            throw new ValidationException("快递单列表不能为空，请先选择快递单号转运！");
        }

        foreach (ExpressInfo express in expressList) {
            if (express.Status != ExpressStatus.CheckIn) {
                throw new ValidationException($"快递单号#{express.TrackingNo}不是已入库状态，快递单选择选择数据有误！");
            }
        }

        var expressGoodsList = session.Query<ExpressGoodsInfo>()
            .Fetch(e => e.Goods)
            .Where(e => expressIdList.Contains(e.Express.Id))
            .ToList();

        IDictionary<int, IList<ExpressGoodsInfo>> expressGoodsListMap = new Dictionary<int, IList<ExpressGoodsInfo>>();
        // 让每个 express id 都有记录
        foreach (int expressId in expressIdList) {
            expressGoodsListMap[expressId] = new List<ExpressGoodsInfo>();
        }

        foreach (ExpressGoodsInfo expressGoods in expressGoodsList) {
            int expressId = expressGoods.Express.Id;

            if (expressGoodsListMap.TryGetValue(expressId, out IList<ExpressGoodsInfo>? thisExpressGoodsList) == false) {
                thisExpressGoodsList = new List<ExpressGoodsInfo>();
            }

            thisExpressGoodsList.Add(expressGoods);

            expressGoodsListMap[expressId] = thisExpressGoodsList;
        }

        // 快递单备注，客户备注，仓库备注，客服备注
        var expressRemarkList = session.Query<ExpressRemarkInfo>()
            .Where(e => expressIdList.Contains(e.Express!.Id))
            .ToList();

        var expressItemList = new List<OrderCreateSummaryExpressListItem>();
        foreach (ExpressInfo express in expressList) {
            int expressId = express.Id;

            string customerRemark = expressRemarkList
                .Where(e => e.Express != null
                            && e.Express.Id == expressId
                            && e.Type == ExpressRemarkType.Customer)
                .Take(1)
                .SingleOrDefault()?.Remark ?? string.Empty;

            string warehouseRemark = expressRemarkList
                .Where(e => e.Express != null
                            && e.Express.Id == expressId
                            && e.Type == ExpressRemarkType.Warehouse)
                .Take(1)
                .SingleOrDefault()?.Remark ?? string.Empty;

            string serviceRemark = expressRemarkList
                .Where(e => e.Express != null
                            && e.Express.Id == expressId
                            && e.Type == ExpressRemarkType.Service)
                .Take(1)
                .SingleOrDefault()?.Remark ?? string.Empty;

            var item = new OrderCreateSummaryExpressListItem {
                Id = express.Id,
                TrackingNo = express.TrackingNo,
                ExpressCompany = express.ExpressCompany,
                CheckinTrackingNo = express.CheckinTrackingNo,
                GoodsType = express.GoodsType.Name,
                AddDate = express.AddTime.ToString("yyyy-MM-dd"),
                CustomerRemark = customerRemark,
                WarehouseRemark = warehouseRemark,
                ServiceRemark = serviceRemark,
            };

            // 一般这个判断肯定是成立的
            if (expressGoodsListMap.TryGetValue(expressId, out IList<ExpressGoodsInfo>? thisExpressGoodList)) {
                if (thisExpressGoodList.Count == 0) {
                    throw new ValidationException($"快递单号#{express.TrackingNo}物品列表为空，数据有问题！");
                }

                IList<OrderCreateSummaryExpressGoodsListItem> goodsList = thisExpressGoodList
                    .Select(e => new OrderCreateSummaryExpressGoodsListItem {
                        Id = e.Id,
                        GoodsName = e.Goods.Name,
                        Quantity = e.Quantity,
                        Price = e.Price.ToString("#0.00"),
                        Unit = e.Unit,
                    })
                    .ToList();

                item.GoodsList = goodsList;
                item.GoodsCount = goodsList.Count;
            }

            expressItemList.Add(item);
        }

        var goodsType = logisticsGoodsTypeService.GetGoodsTypeFromGoodsTypeList(
            expressList.Select(e => e.GoodsType).Distinct().ToList()
        );

        var totalPackageCount = expressList.Sum(e => e.PackageCount);
        decimal totalGoodsValue = expressGoodsList.Sum(e => e.Price * e.Quantity);

        #endregion

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new OrderCreateSummaryModel {
                ExpressList = expressItemList,
                GoodsType = goodsType.Name,
                TotalPackageCount = totalPackageCount,
                TotalGoodsValue = totalGoodsValue.ToString("#0.00"),
            },
        });
    }
}