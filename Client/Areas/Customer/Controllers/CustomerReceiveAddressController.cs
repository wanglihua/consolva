using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Customer.Models.CustomerReceiveAddress;
using shunda.Client.Services;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain;
using shunda.Domain.Customer;
using shunda.Service.Customer;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Customer.Controllers;

[CustomerArea]
public class CustomerReceiveAddressController : Controller {
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;
    private readonly CustomerReceiveAddressService customerReceiveAddressService;

    public CustomerReceiveAddressController(ISession session,
        LoginCustomerService loginCustomerService,
        CustomerReceiveAddressService customerReceiveAddressService
    ) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
        this.customerReceiveAddressService = customerReceiveAddressService;
    }

    [HttpPost]
    [Route("/client/customer/receive/address/list")]
    public IActionResult List([FromBody] CustomerReceiveAddressListParam param) {
        var model = GetListData(param);

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = model,
        });
    }

    [NonAction]
    private CustomerReceiveAddressListModel GetListData(CustomerReceiveAddressListParam param) {
        int loginCustomerId = loginCustomerService.GetLoginCustomerId();
        var query = session.Query<CustomerReceiveAddressInfo>()
            .Where(e => e.Customer.Id == loginCustomerId);

        var receiveAddressList = query
            .OrderByDescending(e => e.Id)
            .ToList();
        var receiveAddressItemList = receiveAddressList
            .Select(e => new CustomerReceiveAddressListItem {
                Id = e.Id,
                Country = e.Country.Name,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Mobile = e.Mobile,
                Address = e.Address,
                City = e.City,
                State = e.State,
                Zipcode = e.Zipcode,
                Tfn = e.Tfn,
                Company = e.Company,
                UnitLoc = e.UnitLoc,
                IsDefault = e.IsDefault,
                FullAddress = customerReceiveAddressService.GetFullAddress(e),
            })
            .ToList();

        int receiveAddressCount = receiveAddressList.Count();

        int sendAddressCount = session.Query<CustomerSendAddressInfo>()
            .Where(e => e.Customer.Id == loginCustomerId)
            .Count();

        return new CustomerReceiveAddressListModel {
            ReceiveAddressList = receiveAddressItemList,
            ReceiveAddressCount = receiveAddressCount,
            SendAddressCount = sendAddressCount,
        };
    }

    [HttpPost]
    [Route("/client/customer/receive/address/detail")]
    public IActionResult Detail([FromBody] IdParam param) {
        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        int addressId = param.Id;
        CustomerReceiveAddressInfo address;
        if (addressId == 0) {
            address = new CustomerReceiveAddressInfo();
        } else {
            address = session.Get<CustomerReceiveAddressInfo>(addressId);
            if (address == null) {
                throw new ValidationException($"指定的收货地址#{addressId}不存在！");
            }

            if (address.Customer.Id != loginCustomerId) {
                throw new ValidationException($"指定的收货地址#{addressId}不属于当前登录客户#{loginCustomerId}");
            }
        }

        var model = new CustomerReceiveAddressDetailModel {
            Id = address.Id,
            CountryId = address.Country.Id,
            FirstName = address.FirstName,
            LastName = address.LastName,
            Mobile = address.Mobile,
            Address = address.Address,
            City = address.City,
            State = address.State,
            Zipcode = address.Zipcode,
            Tfn = address.Tfn,
            Company = address.Company,
            UnitLoc = address.UnitLoc,

            CountryList = Country.GetNoChinaList(),
        };

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = model,
        });
    }

    [HttpPost]
    [Route("/client/customer/receive/address/save")]
    [Transactional]
    public IActionResult Save([FromBody] CustomerReceiveAddressSaveModel model) {
        if (ModelState.IsValid == false) {
            throw new ValidationException(ValidationUtil.GetErrorMessagesForMobile(ModelState));
        }

        Country country = Country.GetById(model.CountryId);
        if (country == Country.None || country == Country.China) {
            throw new ValidationException("请选择正确的国家选项！");
        }

        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        int addressId = model.Id;
        CustomerReceiveAddressInfo address;
        if (addressId == 0) {
            address = new CustomerReceiveAddressInfo();
        } else {
            address = session.Get<CustomerReceiveAddressInfo>(addressId);
            if (address == null) {
                throw new ValidationException($"指定的收货地址#{addressId}不存在！");
            }

            if (address.Customer.Id != loginCustomerId) {
                throw new ValidationException($"指定的收货地址#{addressId}不属于当前登录客户#{loginCustomerId}");
            }
        }

        address.Country = country;
        address.FirstName = model.FirstName;
        address.LastName = model.LastName;
        address.Mobile = model.Mobile;
        address.Address = model.Address;
        address.City = model.City;
        address.State = model.State;
        address.Zipcode = model.Zipcode;
        address.Tfn = model.Tfn;
        address.Company = model.Company;
        address.UnitLoc = model.UnitLoc;

        session.SaveOrUpdate(address);
        session.Flush();

        return Json(new JsonResultData {
            Success = true,
            Message = "收货地址保存成功！",
        });
    }

    [HttpPost]
    [Route("/client/customer/receive/address/set/default")]
    [Transactional]
    public IActionResult SetDefault([FromBody] CustomerReceiveAddressSetDefaultParam param) {
        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        int addressId = param.Id;
        var address = session.Get<CustomerReceiveAddressInfo>(addressId);
        if (address == null) {
            throw new ValidationException($"指定的收货地址#{addressId}不存在！");
        }

        if (address.Customer.Id != loginCustomerId) {
            throw new ValidationException($"指定的收货地址#{addressId}不属于当前登录客户#{loginCustomerId}");
        }

        // 将其他的有默认标记的地址，设置成非默认地址
        // 一般应该也只有一个
        var addressDefaultList = session.Query<CustomerReceiveAddressInfo>()
            .Where(e => e.Customer.Id == loginCustomerId && e.IsDefault == true)
            .ToList();
        foreach (var addressDefault in addressDefaultList) {
            addressDefault.IsDefault = false;
            session.Update(addressDefault);
        }

        session.Flush();

        // 设置当前为默认
        address.IsDefault = true;
        session.Update(address);

        session.Flush();

        var model = GetListData(param);

        return Json(new JsonResultData {
            Success = true,
            Message = "设置默认收货地址成功！",
            Data = model,
        });
    }

    [HttpPost]
    [Route("/client/customer/receive/address/delete")]
    [Transactional]
    public IActionResult Delete([FromBody] CustomerReceiveAddressDeleteParam param) {
        if (ModelState.IsValid == false) {
            throw new ValidationException(ValidationUtil.GetErrorMessagesForMobile(ModelState));
        }

        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        int addressId = param.Id;
        var address = session.Get<CustomerReceiveAddressInfo>(addressId);
        if (address == null) {
            throw new ValidationException($"指定的收货地址#{addressId}不存在！");
        }

        if (address.Customer.Id != loginCustomerId) {
            throw new ValidationException($"指定的收货地址#{addressId}不属于当前登录客户#{loginCustomerId}");
        }

        session.Delete(address);
        session.Flush();

        var model = GetListData(param);

        return Json(new JsonResultData {
            Success = true,
            Message = "收货地址删除成功！",
            Data = model,
        });
    }
}