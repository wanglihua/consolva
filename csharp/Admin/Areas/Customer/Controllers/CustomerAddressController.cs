using Microsoft.AspNetCore.Mvc;
using NHibernate.Criterion;
using shunda.Core;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Customer;
using shunda.Domain.Pass.PassRes;
using Web.Areas.Admin.Models.CustomerAddress;
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

    public CustomerAddressController(ISession session)
    {
        this.session = session;
    }

    [HttpGet]
    [Route("/admin/customer/address/{customerId:int}")]
    public IActionResult Index(int customerId)
    {
        CustomerInfo customerInfo = session.Get<CustomerInfo>(customerId);
        if (customerInfo == null)
        {
            return Content("指定的客户Id不存在！");
        }

        IList<CustomerAddressInfo> customerAddressList = session.CreateCriteria<CustomerAddressInfo>()
            .Add(Restrictions.Where<CustomerAddressInfo>(e => e.Customer.Id == customerId))
            .AddOrder(new Order(nameof(CustomerAddressInfo.IsDefault), false)) // 默认地址排在前面
            .AddOrder(new Order(nameof(CustomerAddressInfo.Id), true))
            .List<CustomerAddressInfo>();

        CustomerAddressIndexModel model = new CustomerAddressIndexModel();
        model.Customer = customerInfo;
        model.CustomerAddressList = customerAddressList;
        model.CustomerAddressCount = customerAddressList.Count;

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

        address.Consignee = addressFromUi.Consignee;
        address.LastName = addressFromUi.LastName;
        address.Mobile = addressFromUi.Mobile;
        address.UnitLoc = addressFromUi.UnitLoc;
        address.Valid = addressFromUi.Valid;
        address.Address = addressFromUi.Address;
        address.City = addressFromUi.City;
        address.Province = addressFromUi.Province;
        address.Zipcode = addressFromUi.Zipcode;
        address.Tfn = addressFromUi.Tfn;
        address.Company = addressFromUi.Company;

        session.Update(address);
        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "保存成功！" },
        });
    }
}