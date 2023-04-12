using Microsoft.AspNetCore.Mvc;
using NHibernate.Criterion;
using shunda.Admin.Areas.Customer.Models.CustomerAddress;
using shunda.Core;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Customer;
using shunda.Domain.Pass.PassRes;
using shunda.Service.Customer;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Customer.Controllers;

[CustomerArea]
[AdminAuth(
    PassRes.Customer.CustomerList
    // todo 后面补上
    // PassRes.LogisticsOrder.OrderListActive,
    // PassRes.LogisticsOrder.OrderListComplete,
    // PassRes.LogisticsOrder.CancelledOrder
)]
public class CustomerAddressController : Controller {
    private readonly ISession session;
    private readonly CustomerCodeService customerCodeService;
    private readonly CustomerReceiveAddressService customerReceiveAddressService;

    public CustomerAddressController(
        ISession session,
        CustomerCodeService customerCodeService,
        CustomerReceiveAddressService customerReceiveAddressService) {
        this.session = session;
        this.customerCodeService = customerCodeService;
        this.customerReceiveAddressService = customerReceiveAddressService;
    }

    [HttpGet]
    [Route("/admin/customer/address/{customerId:int}")]
    public IActionResult Index(int customerId) {
        CustomerInfo? customer = session.Get<CustomerInfo>(customerId);
        if (customer == null) {
            return Content("指定的客户Id不存在！");
        }

        customer.CustomerCodeCache = customerCodeService.GetCustomerCode(customer.Id);

        IList<CustomerReceiveAddressInfo> customerReceiveAddressList = session.CreateCriteria<CustomerReceiveAddressInfo>()
            .Add(Restrictions.Where<CustomerReceiveAddressInfo>(e => e.Customer.Id == customerId))
            .AddOrder(new Order(nameof(CustomerReceiveAddressInfo.IsDefault), false)) // 默认地址排在前面
            .AddOrder(new Order(nameof(CustomerReceiveAddressInfo.Id), true))
            .List<CustomerReceiveAddressInfo>();

        IList<CustomerSendAddressInfo> customerSendAddressList = session.CreateCriteria<CustomerSendAddressInfo>()
            .Add(Restrictions.Where<CustomerSendAddressInfo>(e => e.Customer.Id == customerId))
            .AddOrder(new Order(nameof(CustomerSendAddressInfo.IsDefault), false)) // 默认地址排在前面
            .AddOrder(new Order(nameof(CustomerSendAddressInfo.Id), true))
            .List<CustomerSendAddressInfo>();

        CustomerAddressIndexModel model = new CustomerAddressIndexModel();
        model.Customer = customer;
        model.CustomerReceiveAddressList = customerReceiveAddressList;
        model.CustomerSendAddressList = customerSendAddressList;

        return View("CustomerAddressIndex", model);
    }

    [HttpPost]
    [Route("/admin/customer/address/receive/detail")]
    public IActionResult ReceiveDetail(int id) {
        var address = session.Get<CustomerReceiveAddressInfo>(id);
        if (address == null) {
            throw new ValidationException($"指定的客户地址#{id}不存在！");
        }

        CustomerAddressReceiveDetailModel model = new CustomerAddressReceiveDetailModel {
            Id = address.Id,
            CustomerId = address.Customer.Id,
            Country = address.Country,
            FirstName = address.FirstName,
            LastName = address.LastName,
            Mobile = address.Mobile,
            State = address.State,
            City = address.City,
            Address = address.Address,
            UnitLoc = address.UnitLoc,
            Zipcode = address.Zipcode,
            Tfn = address.Tfn,
            Company = address.Company,
        };

        return View("CustomerAddressReceiveDetail", model);
    }

    [HttpPost]
    [Route("/admin/customer/address/receive/save")]
    public IActionResult ReceiveSave(CustomerAddressReceiveSaveModel model) {
        ValidationUtil.Validate<CustomerAddressReceiveSaveModelValidator, CustomerAddressReceiveSaveModel>(model);

        int addressId = model.Id;
        CustomerReceiveAddressInfo? address = session.Get<CustomerReceiveAddressInfo>(addressId);
        if (address == null) {
            throw new ValidationException($"指定的客户地址#{addressId}不存在！");
        }

        int customerId = model.CustomerId;
        var customer = session.Get<CustomerInfo>(customerId);
        if (customer == null) {
            throw new ValidationException($"指定的客户#{customerId}不存在！");
        }

        address.Customer = customer;
        address.Country = model.Country;
        address.FirstName = model.FirstName;
        address.LastName = model.LastName;
        address.Mobile = model.Mobile;
        address.UnitLoc = model.UnitLoc;
        address.Address = model.Address;
        address.City = model.City;
        address.State = model.State;
        address.Zipcode = model.Zipcode;
        address.Tfn = model.Tfn;
        address.Company = model.Company;

        customerReceiveAddressService.CheckAddress(address);

        session.Update(address);
        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "保存成功！" },
        });
    }
}