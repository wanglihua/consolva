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
using shunda.Service.Logistics;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Express.Controllers;

[ExpressArea]
public class ExpressClaimController : Controller {
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;
    private readonly ExpressTrackingNoService expressTrackingNoService;
    private readonly ExpressClaimService expressClaimService;
    private readonly LogisticsModeSettingService logisticsModeSettingService;

    public ExpressClaimController(ISession session,
        LoginCustomerService loginCustomerService,
        ExpressTrackingNoService expressTrackingNoService,
        ExpressClaimService expressClaimService,
        LogisticsModeSettingService logisticsModeSettingService) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
        this.expressTrackingNoService = expressTrackingNoService;
        this.expressClaimService = expressClaimService;
        this.logisticsModeSettingService = logisticsModeSettingService;
    }

    [HttpPost]
    [Route("/client/express/claim/list")]
    public IActionResult List([FromBody] ExpressClaimListParam param) {
        // 这里只显示面单客户ID不能解析获得的快递单

        var listQuery = GetListQuery(param);

        if (param.LastId != ClientListLastIdModel.LAST_ID_NOT_SET_VALUE) {
            // 跟降序排序匹配
            int lastIdQuery = param.LastId;
            listQuery = listQuery.Where(e => e.Id < lastIdQuery); // 降序排序id大的会被先取得
        }

        IList<ExpressInfo> expressClaimList = listQuery
            .OrderByDescending(e => e.Id) // 按Id降序排序
            .Take(15) // 每次下拉取s15个
            .ToList();

        IList<int> expressIdList = expressClaimList.Select(e => e.Id).ToList();
        IList<ExpressRemarkInfo> expressRemarkList = session.Query<ExpressRemarkInfo>()
            .Where(e => expressIdList.Contains(e.Express!.Id))
            .ToList();

        IList<ExpressClaimListItem> expressClaimItemList = new List<ExpressClaimListItem>();
        foreach (ExpressInfo express in expressClaimList) {
            int expressId = express.Id;

            var expressRemark = expressRemarkList
                .Where(e => e.Express!.Id == expressId && e.Type == ExpressRemarkType.Warehouse)
                .Take(1)
                .SingleOrDefault();
            string warehouseRemark = expressRemark?.Content ?? string.Empty;

            expressClaimItemList.Add(new ExpressClaimListItem() {
                Id = expressId,
                BillCustomerCode = express.BillCustomerCode,
                TrackingNo = expressTrackingNoService.GetHiddenTrackingNo(express.TrackingNo),
                CheckinTrackingNo = express.CheckinTrackingNo,
                GoodsType = express.GoodsType.Name,
                GoodsTypeId = express.GoodsType.Id,
                AddDate = express.AddTime.ToString("yyyy-MM-dd"),
                ExpressCompany = express.ExpressCompany,
                WarehouseRemark = warehouseRemark,
            });
        }

        // 取最末尾，其实降序时就是最小的那一个Id
        var lastData = expressClaimList.LastOrDefault();
        // 这里的 1 很重要，不要取0。如果取值1的话也不破坏逻辑，1的时候就正好就是没数据了，查询时 <1 也是数据
        int lastId = lastData?.Id ?? ClientListLastIdModel.LAST_ID_NO_MORE_DATA_VALUE;

        int expressClaimCount = GetListQuery(param).Count();

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new ExpressClaimListModel() {
                ExpressClaimCount = expressClaimCount,
                ExpressClaimList = expressClaimItemList,
                LastId = lastId,
            }
        });
    }

    [NonAction]
    private IQueryable<ExpressInfo> GetListQuery(ExpressClaimListParam param) {
        var query = session.Query<ExpressInfo>()
            .Where(e => e.BillCustomerId == 0
                        && e.Status == ExpressStatus.WaitPick);

        if (string.IsNullOrWhiteSpace(param.Query) == false) {
            string queryStr = param.Query.Trim();
            query = query.Where(e => e.TrackingNo == queryStr || e.CheckinTrackingNo == queryStr);
        }

        return query;
    }

    [HttpPost]
    [Route("/client/express/claim/detail")]
    public IActionResult Detail([FromBody] IdParam param) {
        if (ModelState.IsValid == false) {
            throw new ValidationException(ValidationUtil.GetErrorMessagesForMobile(ModelState));
        }

        int expressId = param.Id;
        var express = session.Get<ExpressInfo>(expressId);
        if (express == null) {
            throw new ValidationException($"指定的申领件#{expressId}不存在！");
        }

        string trackingNo = express.TrackingNo;

        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        bool claimed = ExpressStatus.IsCheckedIn(express.Status);
        if (claimed && loginCustomerId != (express.Customer?.Id ?? 0)) {
            throw new ValidationException("指定的申领件已经被认领，且认领人不是当前登录客户！");
        }

        // 查看有没有当前登录客户的相同单号的预报
        var expressExpect = session.Query<ExpressExpectInfo>()
            .Where(e => e.Customer.Id == loginCustomerId && e.TrackingNo == trackingNo)
            .Take(1)
            .SingleOrDefault();

        ExpressClaimDetailModel model = new ExpressClaimDetailModel() {
            Id = express.Id,
            TrackingNo = expressTrackingNoService.GetHiddenTrackingNo(trackingNo),
            AddDate = express.AddTime.ToString("yyyy-MM-dd"),
            AddTime = express.AddTime.ToString("yyyy-MM-dd HH:mm"),
            ExpressCompany = express.ExpressCompany,
            BillCustomerCode = express.BillCustomerCode,
            GoodsType = express.GoodsType.Name,
            GoodsValue = express.GoodsValue > 0 ? express.GoodsValue.ToString("#0.00") : string.Empty,
            CheckinTrackingNo = express.CheckinTrackingNo,
            ExpectTrackingNo = expressExpect?.TrackingNo ?? string.Empty,
            ExpectDescription = expressExpect?.Description ?? string.Empty,
            Claimed = claimed,
        };

        // 入库备注
        var expressWarehouseRemark = session.Query<ExpressRemarkInfo>()
            .Where(e => e.Express!.Id == expressId
                        && e.Type == ExpressRemarkType.Warehouse)
            .Take(1)
            .SingleOrDefault();
        model.WarehouseRemark = expressWarehouseRemark?.Content ?? string.Empty;

        // 物品列表
        var expressGoodsList = session.Query<ExpressGoodsInfo>()
            .Fetch(e => e.Goods)
            .Where(e => e.Express.Id == expressId)
            .OrderBy(e => e.Id) // 快递单号物品都按id升序排
            .ToList();

        var expressGoodsItemList = new List<ExpressClaimDetailModel.ExpressGoodsItem>();
        foreach (ExpressGoodsInfo expressGoods in expressGoodsList) {
            expressGoodsItemList.Add(new ExpressClaimDetailModel.ExpressGoodsItem() {
                Id = expressGoods.Id,
                ExpressId = expressGoods.Express.Id,
                GoodsId = expressGoods.Goods.Id,
                GoodsName = expressGoods.Goods.Name,
                Quantity = expressGoods.Quantity,
                Unit = expressGoods.Unit,
            });
        }

        model.ExpressGoodsList = expressGoodsItemList;

        // 尺寸重量
        var expressSizeWeightList = session.Query<ExpressSizeWeightInfo>()
            // .Where(e => e.Express!.Id == expressId)
            .Where(e => e.TrackingNo == trackingNo)
            .OrderBy(e => e.Id)
            .ToList();

        decimal totalVolume = 0;
        decimal totalVolumeWeight = 0;
        decimal totalActualWeight = 0;
        int totalPackageCount = 0;

        decimal volumeWeightDenominator = logisticsModeSettingService.GetSetting().VolumeWeightDenominator;

        var expressSizeWeightItemList = new List<ExpressClaimDetailModel.ExpressSizeWeightItem>();
        foreach (ExpressSizeWeightInfo sizeWeight in expressSizeWeightList) {
            totalVolume += sizeWeight.GetSingleVolumeCache();
            totalVolumeWeight += sizeWeight.GetSingleVolumeWeightCache(volumeWeightDenominator);
            totalActualWeight += sizeWeight.Weight;
            totalPackageCount += sizeWeight.PackageCount;

            expressSizeWeightItemList.Add(new ExpressClaimDetailModel.ExpressSizeWeightItem() {
                Id = sizeWeight.Id,
                Length = sizeWeight.Length.ToString("#0.00"),
                Width = sizeWeight.Width.ToString("#0.00"),
                Height = sizeWeight.Height.ToString("#0.00"),
                Weight = sizeWeight.Weight.ToString("#0.000"),
                Volume = sizeWeight.GetSingleVolumeCache().ToString("#0.000"),
                VolumeWeight = sizeWeight.GetSingleVolumeCache().ToString("#0.000"),
                PackageCount = sizeWeight.PackageCount,
            });
        }

        model.TotalVolume = totalVolume.ToString("#0.000");
        model.TotalVolumeWeight = totalVolumeWeight.ToString("#0.000");
        model.TotalActualWeight = totalActualWeight.ToString("#0.000");
        model.TotalPackageCount = totalPackageCount;
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
    public IActionResult Pick([FromBody] ExpressClaimPickParam param) {
        ValidationUtil.Validate<ExpressClaimPickParamValidator, ExpressClaimPickParam>(param);

        int expressId = param.ExpressId;
        string trackingNoInput = param.TrackingNoInput;

        var express = session.Get<ExpressInfo>(expressId);
        if (express == null) {
            throw new ValidationException($"指定的申领件#{expressId}不存在！");
        }

        if (decimal.TryParse(param.GoodsValueInput, out decimal goodsValue) == false || goodsValue <= 0) {
            throw new ValidationException($"您还未完善快递单号货品总值。请如实申报货值，虚假申报可能会导致高额税金、罚款、销毁物品！");
        }

        express.GoodsValue = goodsValue;

        session.Update(express);
        session.Flush();

        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        expressClaimService.Pick(expressId, trackingNoInput, loginCustomerId);

        return Json(new JsonResultData {
            Success = true,
            Message = "认领成功！",
        });
    }

    [HttpPost]
    [Route("/client/express/claim/match")]
    public IActionResult Match([FromBody] ExpressClaimMatchParam param) {
        if (ModelState.IsValid == false) {
            throw new ValidationException(ValidationUtil.GetErrorMessagesForMobile(ModelState));
        }

        int expressId = param.ExpressId;
        string trackingNoInput = param.TrackingNoInput;

        var express = session.Get<ExpressInfo>(expressId);
        if (express == null) {
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