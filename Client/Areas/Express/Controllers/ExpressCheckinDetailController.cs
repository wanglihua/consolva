using Microsoft.AspNetCore.Mvc;
using NHibernate.Linq;
using shunda.Client.Areas.Express.Models.ExpressCheckin;
using shunda.Client.Areas.Express.Models.ExpressCheckinDetail;
using shunda.Client.Services;
using shunda.Core;
using shunda.Core.Config;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Express;
using shunda.Domain.Logistics;
using shunda.Service.Express;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Express.Controllers;

[ExpressArea]
public class ExpressCheckinDetailController : Controller {
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;
    private readonly ExpressPhotoFileService expressPhotoFileService;

    public ExpressCheckinDetailController(ISession session,
        LoginCustomerService loginCustomerService,
        ExpressPhotoFileService expressPhotoFileService
    ) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
        this.expressPhotoFileService = expressPhotoFileService;
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

        // goods
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

        // size weight
        var sizeWeightList = session.Query<ExpressSizeWeightInfo>()
            .Where(e => e.Express!.Id == expressId)
            .ToList();
        decimal totalVolume = 0m;
        decimal totalVolumeWeight = 0m;
        decimal totalActualWeight = 0m;
        int packageCount = 0;
        if (sizeWeightList.Count != 0) {
            totalVolume = sizeWeightList.Sum(e => e.Length * e.Weight * e.Height);
            totalActualWeight = sizeWeightList.Sum(e => e.Weight);
            packageCount = sizeWeightList.Sum(e => e.PackageCount);
        }

        totalVolumeWeight = totalVolume / LogisticsConfig.VolumeWeightDenominator;
        totalVolume = totalVolume / 100_00_00; // 转成立方米

        var sizeWeightItemList = sizeWeightList
            .OrderBy(e => e.Id)
            .Select(e => new ExpressCheckinDetailSizeWeightListItem {
                Id = e.Id,
                Length = e.Length.ToString("#0.00"),
                Width = e.Width.ToString("#0.00"),
                Height = e.Height.ToString("#0.00"),
                VolumeWeight = (e.Length * e.Width * e.Height / LogisticsConfig.VolumeWeightDenominator).ToString("#0.000"),
                ActualWeight = e.Weight.ToString("#0.000"),
                PackageCount = e.PackageCount,
            })
            .ToList();

        // photo
        var photoList = session.Query<ExpressPhotoInfo>().Where(e => e.Express!.Id == expressId).ToList();
        var photoTypeList = photoList.Select(e => e.Type).Distinct().OrderBy(e => e.Id).ToList();

        var photoTypeItemList = photoTypeList
            .Select(e => new ExpressCheckinDetailPhotoTypeListItem {
                Id = e.Id,
                Name = e.Name,
                PhotoList = photoList
                    .Where(p => p.Type.Id == e.Id)
                    .OrderBy(p => p.Id)
                    .Select(p => new ExpressCheckinDetailPhotoListItem {
                        Id = p.Id,
                        SmallUrl = expressPhotoFileService.GetSmallImageFullUrlForApp(p, OssSiteLocType.HongKong),
                        BigUrl = expressPhotoFileService.GetBigImageFullUrlForApp(p, OssSiteLocType.HongKong)
                    })
                    .ToList(),
            })
            .ToList();

        // model
        ExpressCheckinDetailModel model = new ExpressCheckinDetailModel {
            Id = express.Id,
            TrackingNo = express.TrackingNo,
            ExpressCompany = express.ExpressCompany,
            GoodsType = express.GoodsType.Name,
            Forbidden = express.GoodsType == LogisticsGoodsType.Forbidden,
            CheckinTrackingNo = express.CheckinTrackingNo,
            AddDate = express.AddTime.ToString("yyyy-MM-dd"),
            ClaimDate = express.ClaimTime?.ToString("yyyy-MM-dd") ?? string.Empty,
            WarehouseRemark = warehouseRemark,
            Status = express.Status.Name,
            WaitOrder = express.Status == ExpressStatus.WaitOrder,

            // goods
            GoodsCount = goodsCount,
            GoodsList = goodsItemList,

            // size weight
            SizeWeightList = sizeWeightItemList,
            TotalVolume = totalVolume.ToString("#0.000000"),
            TotalVolumeWeight = totalVolumeWeight.ToString("#0.000"),
            TotalActualWeight = totalActualWeight.ToString("#0.000"),
            TotalPackageCount = packageCount,

            // photo
            PhotoTypeList = photoTypeItemList,
        };

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = model,
        });
    }
}