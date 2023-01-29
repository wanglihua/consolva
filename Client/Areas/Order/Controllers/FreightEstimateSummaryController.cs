using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Order.Models.FreightEstimateSummary;
using shunda.Client.Services;
using shunda.Core;
using shunda.Core.Web;
using shunda.Domain.Customer;
using shunda.Domain.Logistics;
using shunda.Domain.LogisticsOrder;
using shunda.Service.Customer;
using shunda.Service.Fee;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Order.Controllers;

[OrderArea]
public class FreightEstimateSummaryController : Controller {
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;
    private readonly FreightEstimateSelectService freightEstimateSelectService;
    private readonly CustomerReceiveAddressService customerReceiveAddressService;

    public FreightEstimateSummaryController(ISession session,
        LoginCustomerService loginCustomerService,
        FreightEstimateSelectService freightEstimateSelectService,
        CustomerReceiveAddressService customerReceiveAddressService
    ) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
        this.freightEstimateSelectService = freightEstimateSelectService;
        this.customerReceiveAddressService = customerReceiveAddressService;
    }

    [HttpPost]
    [Route("/client/freight/estimate/summary")]
    public IActionResult Summary() {
        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        var estimateSelect = session.Get<FreightEstimateSelectInfo>(loginCustomerId);
        if (estimateSelect == null) {
            throw new ValidationException($"指定客户的运费预估记录数据#{loginCustomerId}不存在！");
        }

        var country = estimateSelect.Country;

        // 物品信息
        var goodsList = session.Query<FreightEstimateSelectGoodsInfo>()
            .Where(e => e.CustomerId == loginCustomerId)
            .ToList();
        if (goodsList.Count == 0) {
            throw new ValidationException($"运费预估物品数据#{loginCustomerId}不存在！");
        }

        var goodsItemList = goodsList
            .Select(e => new FreightEstimateSummaryGoodsListItem {
                Id = e.Id,
                Width = e.Width.ToString("#0.00"),
                Length = e.Length.ToString("#0.00"),
                Height = e.Height.ToString("#0.00"),
                Weight = e.Weight.ToString("#0.00"),
                PackageCount = e.PackageCount.ToString(),
            })
            .ToList();

        // 物品汇总
        var goodsType = estimateSelect.GoodsType.Name;
        var goodsValue = estimateSelect.GoodsValue.ToString("#0.00");
        var totalPackageCount = goodsList.Sum(e => e.PackageCount);

        // 收货地址
        int addressReceiveId = estimateSelect.AddressReceiveId;
        var addressReceive = session.Get<CustomerReceiveAddressInfo>(addressReceiveId);
        if (addressReceive == null) {
            throw new ValidationException($"指定的收货地址#{addressReceiveId}不存在，数据有问题！");
        }

        if (addressReceive.Customer.Id != loginCustomerId) {
            throw new ValidationException($"收货地址#{addressReceiveId}不是当前登录客户的，数据有问题!");
        }

        if (addressReceive.Country != country) {
            throw new ValidationException($"收货地址#{addressReceiveId}与国家选择记录#{country.Name}不匹配，数据有问题!");
        }

        var addressReceiveItem = new FreightEstimateSummaryAddressReceiveItem {
            Id = addressReceive.Id,
            Country = addressReceive.Country.Name,
            FirstName = addressReceive.FirstName,
            LastName = addressReceive.LastName,
            Mobile = addressReceive.Mobile,
            Address = addressReceive.Address,
            City = addressReceive.City,
            State = addressReceive.State,
            Zipcode = addressReceive.Zipcode,
            Tfn = addressReceive.Tfn,
            Company = addressReceive.Company,
            UnitLoc = addressReceive.UnitLoc,
            FullAddress = customerReceiveAddressService.GetFullAddress(addressReceive),
        };

        // 头程物流
        int firstModeId = estimateSelect.FirstModeId;
        var firstMode = session.Get<LogisticsModeInfo>(firstModeId);
        if (firstMode == null) {
            throw new ValidationException($"指定的物流方式#{firstModeId}不存在，数据有问题！");
        }

        if (firstMode.Status != LogisticsModeStatus.Enabled) {
            throw new ValidationException($"指定的物流方式#{firstModeId}已禁用，数据有问题！");
        }

        if (firstMode.Country != country) {
            throw new ValidationException($"物流方式#{firstModeId}与国家选择记录#{country.Name}不匹配，数据有问题!");
        }

        var firstModeItem = new FreightEstimateSummaryFirstModeItem {
            Id = firstMode.Id,
            Country = firstMode.Country.Name,
            Type = firstMode.Type.Name,
            Name = firstMode.Name,
            Description = firstMode.Description,
        };

        // 物流费用

        // GST费用

        // 总费用

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new FreightEstimateSummaryModel {
                GoodsList = goodsItemList,
                GoodsType = goodsType,
                GoodsValue = goodsValue,
                TotalPackageCount = totalPackageCount,
                ReceiveAddress = addressReceiveItem,
                FirstMode = firstModeItem,
            },
        });
    }
}