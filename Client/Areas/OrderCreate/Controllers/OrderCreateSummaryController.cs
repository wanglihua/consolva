using Microsoft.AspNetCore.Mvc;
using NHibernate.Linq;
using shunda.Client.Areas.OrderCreate.Models.OrderCreateSummary;
using shunda.Client.Services;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Web;
using shunda.Domain.Customer;
using shunda.Domain.Express;
using shunda.Domain.Logistics;
using shunda.Domain.LogisticsOrder;
using shunda.Service.Customer;
using shunda.Service.Logistics;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.OrderCreate.Controllers;

[OrderCreateArea]
public class OrderCreateSummaryController : Controller {
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;
    private readonly LogisticsGoodsTypeService logisticsGoodsTypeService;
    private readonly CustomerReceiveAddressService customerReceiveAddressService;

    public OrderCreateSummaryController(ISession session,
        LoginCustomerService loginCustomerService,
        LogisticsGoodsTypeService logisticsGoodsTypeService,
        CustomerReceiveAddressService customerReceiveAddressService
    ) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
        this.logisticsGoodsTypeService = logisticsGoodsTypeService;
        this.customerReceiveAddressService = customerReceiveAddressService;
    }

    [HttpPost]
    [Route("/client/order/create/summary")]
    [Transactional] // 设置成 valid
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
            if (express.Status != ExpressStatus.Checkin) {
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
                .SingleOrDefault()?.Content ?? string.Empty;

            string warehouseRemark = expressRemarkList
                .Where(e => e.Express != null
                            && e.Express.Id == expressId
                            && e.Type == ExpressRemarkType.Warehouse)
                .Take(1)
                .SingleOrDefault()?.Content ?? string.Empty;

            string serviceRemark = expressRemarkList
                .Where(e => e.Express != null
                            && e.Express.Id == expressId
                            && e.Type == ExpressRemarkType.Service)
                .Take(1)
                .SingleOrDefault()?.Content ?? string.Empty;

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
        decimal totalGoodsValue = expressList.Sum(e => e.GoodsValue);

        #endregion

        var createSelect = session.Get<LogisticsOrderCreateSelectInfo>(loginCustomerId);
        if (createSelect == null) {
            throw new ValidationException("当前客户没有订单创建选择记录信息！");
        }

        // 头程物流
        int firstModeId = createSelect.FirstModeId;
        var firstMode = session.Get<LogisticsModeInfo>(firstModeId);
        if (firstMode == null) {
            throw new ValidationException($"指定的物流方式#{firstModeId}不存在！");
        }

        var firstModeItem = new OrderCreateSummaryFirstModeItem {
            Id = firstMode.Id,
            Country = firstMode.Country.Name,
            Type = firstMode.Type.Name,
            Name = firstMode.Name,
            Description = firstMode.Description,
        };

        // 地址
        int receiveAddressId = createSelect.AddressReceiveId;
        var receiveAddress = session.Get<CustomerReceiveAddressInfo>(receiveAddressId);
        if (receiveAddress == null) {
            throw new ValidationException($"指定的收货地址#{receiveAddressId}不存在！");
        }

        var receiveAddressItem = new OrderCreateSummaryReceiveAddressItem {
            Id = receiveAddress.Id,
            Country = receiveAddress.Country.Name,
            FirstName = receiveAddress.FirstName,
            LastName = receiveAddress.LastName,
            Mobile = receiveAddress.Mobile,
            Address = receiveAddress.Address,
            City = receiveAddress.City,
            State = receiveAddress.State,
            Zipcode = receiveAddress.Zipcode,
            Tfn = receiveAddress.Tfn,
            Company = receiveAddress.Company,
            UnitLoc = receiveAddress.UnitLoc,
            FullAddress = customerReceiveAddressService.GetFullAddress(receiveAddress),
        };

        /*
        int sendAddressId = createSelect.AddressSendId;
        var sendAddress = session.Get<CustomerSendAddressInfo>(sendAddressId);
        if (sendAddress == null) {
            throw new ValidationException($"指定的发货地址#{sendAddressId}不存在！");
        }

        var sendAddressItem = new OrderCreateSummarySendAddressItem {
            Id = sendAddress.Id,
            Country = sendAddress.Country.Name,
            Name = sendAddress.Name,
            Mobile = sendAddress.Mobile,
            Province = sendAddress.Province.Name,
            City = sendAddress.City.Name,
            Area = sendAddress.Area.Name,
            Street = sendAddress.Street.Name,
            Address = sendAddress.Address,
            Zipcode = sendAddress.Zipcode,
        };
        */

        // 保险和GST
        int insuranceId = createSelect.InsuranceId;
        var insurance = session.Get<LogisticsInsuranceInfo>(insuranceId);
        if (insurance == null) {
            throw new ValidationException($"指定的保险方案#{insuranceId}不存在！");
        }

        var insuranceItem = new OrderCreateSummaryInsuranceItem {
            Id = insurance.Id,
            Name = insurance.Name,
        };

        int gstId = createSelect.GstId;
        var gst = session.Get<LogisticsGstInfo>(gstId);
        if (gst == null) {
            throw new ValidationException($"指定的GST方案#{gstId}不存在！");
        }

        var gstItem = new OrderCreateSummaryGstItem {
            Id = gst.Id,
            Name = gst.Name,
        };

        int repackTypeId = createSelect.RepackType.Id;
        var repackTypeSetting = session.Query<LogisticsRepackTypeSettingInfo>()
            .Where(e => e.Type.Id == repackTypeId)
            .Take(1)
            .SingleOrDefault();

        var repackTypeItem = new OrderCreateSummaryRepackTypeItem {
            Title = repackTypeSetting?.Title ?? string.Empty,
            Description = repackTypeSetting?.Description ?? string.Empty,
        };

        string remark = createSelect.Remark;

        // 保存 valid
        createSelect.Valid = true;
        session.Update(createSelect);
        session.Flush();

        // todo 订单创建时必须要做必要的数据验证，这里就先不做了

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new OrderCreateSummaryModel {
                ExpressList = expressItemList,
                GoodsType = goodsType.Name,
                TotalPackageCount = totalPackageCount,
                TotalGoodsValue = totalGoodsValue.ToString("#0.00"),
                ReceiveAddress = receiveAddressItem,
                // SendAddress = sendAddressItem,
                FirstMode = firstModeItem,
                Insurance = insuranceItem,
                Gst = gstItem,
                RepackType = repackTypeItem,
                Remark = remark,
            },
        });
    }
}