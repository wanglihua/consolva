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
public class CustomerAddressController : Controller
{
    private ISession session;
    private CustomerCodeService customerCodeService;

    public CustomerAddressController(ISession session, CustomerCodeService customerCodeService)
    {
        this.session = session;
        this.customerCodeService = customerCodeService;
    }

    [HttpGet]
    [Route("/admin/customer/address/{customerId:int}")]
    public IActionResult Index(int customerId)
    {
        CustomerInfo customer = session.Get<CustomerInfo>(customerId);
        if (customer == null)
        {
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
        model.CustomerReceiveAddressCount = customerReceiveAddressList.Count;
        model.CustomerSendAddressList = customerSendAddressList;
        model.CustomerSendAddressCount = customerSendAddressList.Count;

        return View("CustomerAddressIndex", model);
    }

    [HttpPost]
    [Route("/admin/customer/address/receive/detail")]
    public IActionResult ReceiveDetail(int id)
    {
        CustomerReceiveAddressInfo customerAddress = session.Get<CustomerReceiveAddressInfo>(id);
        if (customerAddress == null)
        {
            throw new ValidationException($"指定的客户地址#{id}不存在！");
        }

        CustomerAddressReceiveDetailModel model = new CustomerAddressReceiveDetailModel();

        model.CustomerAddress = customerAddress;

        return View("CustomerAddressReceiverDetail", model);
    }

    [HttpPost]
    [Route("/admin/customer/address/receive/save")]
    public IActionResult ReceiveSave(CustomerAddressReceiveDetailModel model)
    {
        CustomerReceiveAddressInfo addressFromUi = model.CustomerAddress;

        if (!this.ModelState.IsValid)
        {
            throw new ValidationException(ValidationUtil.GetErrorMessages(this.ModelState));
        }

        int addressId = addressFromUi.Id;
        CustomerReceiveAddressInfo address = session.Get<CustomerReceiveAddressInfo>(addressId);
        if (address == null)
        {
            throw new ValidationException($"指定的客户地址#{addressId}不存在！");
        }

        address.Country = addressFromUi.Country;
        address.FirstName = addressFromUi.FirstName;
        address.LastName = addressFromUi.LastName;
        address.Mobile = addressFromUi.Mobile;
        address.UnitLoc = addressFromUi.UnitLoc;
        address.Address = addressFromUi.Address;
        address.City = addressFromUi.City;
        address.State = addressFromUi.State;
        address.Zipcode = addressFromUi.Zipcode;
        address.Tfn = addressFromUi.Tfn;
        address.Company = addressFromUi.Company;
        // address.IsDefault = addressFromUi.IsDefault;
        // address.Status = addressFromUi.Status;

        session.Update(address);
        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "保存成功！" },
        });
    }
}