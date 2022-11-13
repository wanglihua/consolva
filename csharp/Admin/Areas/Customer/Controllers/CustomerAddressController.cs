using System.Collections.Generic;
using Core;
using Core.Util;
using Domain;
using Domain.Customer;
using Domain.Pass;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using Web.Areas.Admin.Models.CustomerAddress;

namespace Web.Areas.Admin.Controllers
{
    [AdminArea]
    [AdminAuth(
        PassRes.Customer.CustomerList,
        PassRes.LogisticsOrder.OrderListActive,
        PassRes.LogisticsOrder.OrderListComplete,
        PassRes.LogisticsOrder.CancelledOrder
    )]
    public class CustomerAddressController : Controller
    {
        private ISession session = null;

        public CustomerAddressController(ISession session)
        {
            this.session = session;
        }

        [HttpGet]
        [Route("/admin/customer/address/{customerId:int}")]
        [NoHibernateAutoTransaction]
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
        [Route("/admin/customer/address/receiver/detail")]
        [NoHibernateAutoTransaction]
        public IActionResult ReceiverDetail(int id)
        {
            CustomerAddressInfo customerAddress = session.Get<CustomerAddressInfo>(id);
            if (customerAddress == null)
            {
                throw new ValidationException($"指定的客户地址#{id}不存在！");
            }

            CustomerAddressReceiverDetailModel model = new CustomerAddressReceiverDetailModel();

            model.AustraliaStateList = AustraliaState.GetList();
            model.CustomerAddress = customerAddress;

            return View("CustomerAddressReceiverDetail", model);
        }

        [HttpPost]
        [Route("/admin/customer/address/receiver/save")]
        public IActionResult ReceiverSave(CustomerAddressReceiverDetailModel model)
        {
            CustomerAddressInfo addressFromUi = model.CustomerAddress;

            if (!this.ModelState.IsValid)
            {
                throw new ValidationException(ValidationUtil.GetErrorMessages(this.ModelState));
            }

            int addressId = addressFromUi.Id;
            CustomerAddressInfo address = session.Get<CustomerAddressInfo>(addressId);
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
}