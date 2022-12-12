using Microsoft.AspNetCore.Mvc;
using NHibernate.Linq;
using shunda.Client.Areas.Express.Models.ExpressClaim;
using shunda.Client.Services;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Express;
using shunda.Service.Express;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Express.Controllers;

[ExpressArea]
public class ExpressClaimController : Controller
{
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;
    private readonly ExpressTrackingNoService expressTrackingNoService;
    private readonly ExpressClaimService expressClaimService;

    public ExpressClaimController(ISession session,
        LoginCustomerService loginCustomerService,
        ExpressTrackingNoService expressTrackingNoService,
        ExpressClaimService expressClaimService)
    {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
        this.expressTrackingNoService = expressTrackingNoService;
        this.expressClaimService = expressClaimService;
    }

    [HttpPost]
    [Route("/client/express/claim/list")]
    public IActionResult List([FromBody] ExpressClaimListParam param)
    {
        // 这里只显示面单客户ID不能解析获得的快递单

        var listQuery = GetListQuery(param);

        if (param.LastExpressId != 0)
        {
            // 跟降序排序匹配
            int lastExpressIdQuery = param.LastExpressId;
            listQuery = listQuery.Where(e => e.Id < lastExpressIdQuery); // 降序排序id大的会被先取得
        }

        IList<ExpressInfo> expressClaimList = listQuery
            .OrderByDescending(e => e.Id) // 按Id降序排序
            .Take(15) // 每次下拉去15个
            .ToList();

        IList<int> expressIdList = expressClaimList.Select(e => e.Id).ToList();
        IList<ExpressRemarkInfo> expressRemarkList = session.Query<ExpressRemarkInfo>()
            .Where(e => expressIdList.Contains(e.Express!.Id))
            .ToList();

        IList<ExpressClaimListItem> expressClaimItemList = new List<ExpressClaimListItem>();
        foreach (ExpressInfo express in expressClaimList)
        {
            int expressId = express.Id;

            var expressRemark = expressRemarkList
                .Where(e => e.Express!.Id == expressId && e.Type == ExpressRemarkType.Warehouse)
                .Take(1)
                .SingleOrDefault();
            string warehouseRemark = expressRemark?.Remark ?? string.Empty;

            expressClaimItemList.Add(new ExpressClaimListItem() {
                Id = expressId,
                BillCustomerCode = express.BillCustomerCode,
                TrackingNo = expressTrackingNoService.GetHiddenTrackingNo(express.TrackingNo),
                CheckinTrackingNo = express.CheckinTrackingNo,
                GoodsType = express.GoodsType.Name,
                AddDate = express.AddTime.ToString("yyyy-MM-dd"),
                ExpressCompany = express.ExpressCompany,
                WarehouseRemark = warehouseRemark,
            });
        }

        // 取最末尾，其实降序时就是最小的那一个Id
        var lastExpressClaim = expressClaimList.LastOrDefault();
        // 这里的 1 很重要，不要取0。如果取值1的话也不破坏逻辑，1的时候就正好就是没数据了，查询时 <1 也是数据
        int lastExpressId = lastExpressClaim?.Id ?? 1;

        int expressClaimCount = GetListQuery(param).Count();

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new ExpressClaimListModel() {
                ExpressClaimCount = expressClaimCount,
                ExpressClaimList = expressClaimItemList,
                LastExpressId = lastExpressId,
            }
        });
    }

    [NonAction]
    private IQueryable<ExpressInfo> GetListQuery(ExpressClaimListParam param)
    {
        var query = session.Query<ExpressInfo>()
            .Where(e => e.BillCustomerId == 0
                        && e.Status == ExpressStatus.WaitPick);

        if (string.IsNullOrWhiteSpace(param.Query) == false)
        {
            string queryStr = param.Query.Trim();
            query = query.Where(e => e.TrackingNo.Contains(queryStr) || e.CheckinTrackingNo.Contains(queryStr));
        }

        return query;
    }

    [HttpPost]
    [Route("/client/express/claim/detail")]
    public IActionResult Detail([FromBody] IdParam param)
    {
        if (ModelState.IsValid == false)
        {
            throw new ValidationException(ValidationUtil.GetErrorMessagesForMobile(ModelState));
        }

        int expressId = param.Id;
        var express = session.Get<ExpressInfo>(expressId);
        if (express == null)
        {
            throw new ValidationException($"指定的申领件#{expressId}不存在！");
        }

        ExpressClaimDetailModel model = new ExpressClaimDetailModel() {
            Id = express.Id,
            TrackingNo = expressTrackingNoService.GetHiddenTrackingNo(express.TrackingNo),
            AddDate = express.AddTime.ToString("yyyy-MM-dd"),
            AddTime = express.AddTime.ToString("yyyy-MM-dd HH:mm"),
            ExpressCompany = express.ExpressCompany,
            BillCustomerCode = express.BillCustomerCode,
            GoodsType = express.GoodsType.Name,
            CheckinTrackingNo = express.CheckinTrackingNo,
        };

        // 入库备注
        var expressWarehouseRemark = session.Query<ExpressRemarkInfo>()
            .Where(e => e.Express!.Id == expressId
                        && e.Type == ExpressRemarkType.Warehouse)
            .Take(1)
            .SingleOrDefault();
        model.WarehouseRemark = expressWarehouseRemark?.Remark ?? string.Empty;

        // 物品列表
        var expressGoodsList = session.Query<ExpressGoodsInfo>()
            .Fetch(e => e.Goods)
            .Where(e => e.Express.Id == expressId)
            .OrderBy(e => e.Id)
            .ToList();

        var expressGoodsItemList = new List<ExpressClaimDetailModel.ExpressGoodsItem>();
        foreach (ExpressGoodsInfo expressGoods in expressGoodsList)
        {
            expressGoodsItemList.Add(new ExpressClaimDetailModel.ExpressGoodsItem() {
                Id = expressGoods.Id,
                ExpressId = expressGoods.Express.Id,
                GoodsId = expressGoods.Goods.Id,
                GoodsName = expressGoods.Goods.Name,
                Price = expressGoods.Price.ToString("#0.00"),
                Quantity = expressGoods.Quantity,
                Unit = expressGoods.Unit,
            });
        }

        model.ExpressGoodsList = expressGoodsItemList;

        // 尺寸重量
        var expressSizeWeightList = session.Query<ExpressSizeWeightInfo>()
            .Where(e => e.Express!.Id == expressId)
            .OrderBy(e => e.Id)
            .ToList();

        decimal totalVolume = 0;
        decimal totalVolumeWeight = 0;
        decimal totalActualWeight = 0;
        int totalPackageCount = 0;
        var expressSizeWeightItemList = new List<ExpressClaimDetailModel.ExpressSizeWeightItem>();
        foreach (ExpressSizeWeightInfo sizeWeight in expressSizeWeightList)
        {
            totalVolume += sizeWeight.GetVolumeCache();
            totalVolumeWeight += sizeWeight.GetVolumeWeightCache();
            totalActualWeight += sizeWeight.Weight;
            totalPackageCount += sizeWeight.PackageCount;

            expressSizeWeightItemList.Add(new ExpressClaimDetailModel.ExpressSizeWeightItem() {
                Id = sizeWeight.Id,
                Length = sizeWeight.Length.ToString("#0.00"),
                Width = sizeWeight.Width.ToString("#0.00"),
                Height = sizeWeight.Height.ToString("#0.00"),
                Weight = sizeWeight.Weight.ToString("#0.000"),
                Volume = sizeWeight.GetVolumeCache().ToString("#0.000"),
                VolumeWeight = sizeWeight.GetVolumeCache().ToString("#0.000"),
            });
        }

        model.TotalVolume = totalVolume.ToString("#0.000");
        model.TotalVolumeWeight = totalVolumeWeight.ToString("#0.000");
        model.TotalActualWeight = totalActualWeight.ToString("#0.000");
        model.TotalPackageCount = totalPackageCount.ToString();
        model.ExpressSizeWeightList = expressSizeWeightItemList;

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = model,
        });
    }

    [HttpPost]
    [Route("/client/express/claim/pick")]
    [Transactional]
    public IActionResult Pick([FromBody] ExpressClaimPickParam param)
    {
        if (ModelState.IsValid == false)
        {
            throw new ValidationException(ValidationUtil.GetErrorMessagesForMobile(ModelState));
        }

        int expressId = param.ExpressId;
        string trackingNoInput = param.TrackingNoInput;
        var goodsPriceList = param.GoodsPriceList;

        var express = session.Get<ExpressInfo>(expressId);
        if (express == null)
        {
            throw new ValidationException($"指定的申领件#{expressId}不存在！");
        }

        if (goodsPriceList == null || goodsPriceList.Count == 0)
        {
            throw new ValidationException("物品价格不能为空！");
        }

        // 检查快递单的物品是不是对有界面上传过来的价格，并且都是大于0的价格
        var expressGoodsList = session.Query<ExpressGoodsInfo>()
            .Where(e => e.Express.Id == expressId)
            .ToList();
        foreach (ExpressGoodsInfo expressGoods in expressGoodsList)
        {
            int expressGoodsId = expressGoods.Id;
            var goodsPrice = goodsPriceList.Where(e => e.ExpressGoodsId == expressGoodsId)
                .Take(1)
                .SingleOrDefault();
            if (goodsPrice == null || goodsPrice.Price <= 0)
            {
                throw new ValidationException($"您还未完善物品 {expressGoods.Goods.Name} 单价。请如实申报货值，虚假申报可能会导致高额税金、罚款、销毁物品！");
            }
        }

        foreach (ExpressGoodsInfo expressGoods in expressGoodsList)
        {
            int expressGoodsId = expressGoods.Id;
            var goodsPrice = goodsPriceList.Where(e => e.ExpressGoodsId == expressGoodsId)
                .Take(1)
                .SingleOrDefault();

            expressGoods.Price = goodsPrice!.Price;

            session.Update(expressGoods);
        }

        session.Flush();

        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        expressClaimService.Pick(expressId, trackingNoInput, loginCustomerId);

        return Json(new JsonResultData {
            Success = true,
            Message = "快递单号认领成功！",
        });
    }

    [HttpPost]
    [Route("/client/express/claim/match")]
    public IActionResult Match([FromBody] ExpressClaimMatchParam param)
    {
        if (ModelState.IsValid == false)
        {
            throw new ValidationException(ValidationUtil.GetErrorMessagesForMobile(ModelState));
        }

        int expressId = param.ExpressId;
        string trackingNoInput = param.TrackingNoInput;

        var express = session.Get<ExpressInfo>(expressId);
        if (express == null)
        {
            throw new ValidationException($"指定的申领件#{expressId}不存在！");
        }

        bool match = expressTrackingNoService.IsTrackingNoMatch(express.TrackingNo, trackingNoInput);

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new ExpressClaimMatchModel() {
                Match = match,
            }
        });
    }
}