using Microsoft.AspNetCore.Mvc;
using NHibernate;
using shunda.Client.Areas.Customer.Models.CustomerSendAddress;
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
public class CustomerSendAddressController : Controller {
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;
    private readonly CustomerSendAddressService customerSendAddressService;

    public CustomerSendAddressController(ISession session,
        LoginCustomerService loginCustomerService,
        CustomerSendAddressService customerSendAddressService
    ) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
        this.customerSendAddressService = customerSendAddressService;
    }

    [HttpPost]
    [Route("/client/customer/send/address/list")]
    public IActionResult List([FromBody] CustomerSendAddressListParam param) {
        var model = GetListData(param);

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = model,
        });
    }

    [NonAction]
    private CustomerSendAddressListModel GetListData(CustomerSendAddressListParam param) {
        int loginCustomerId = loginCustomerService.GetLoginCustomerId();
        var query = session.Query<CustomerSendAddressInfo>()
            .Where(e => e.Customer.Id == loginCustomerId);

        var sendAddressList = query
            .OrderByDescending(e => e.Id)
            .ToList();

        var provinceIdList = sendAddressList.Select(e => e.Province.Id).ToList();
        var cityIdList = sendAddressList.Select(e => e.City.Id).ToList();
        var areaIdList = sendAddressList.Select(e => e.Area.Id).ToList();
        var streetIdList = sendAddressList.Select(e => e.Street.Id).ToList();

        // 预取
        var provinceList = session.Query<AddressProvinceInfo>()
            .Where(e => provinceIdList.Contains(e.Id))
            .ToList();
        var cityList = session.Query<AddressCityInfo>()
            .Where(e => cityIdList.Contains(e.Id))
            .ToList();
        var areaList = session.Query<AddressAreaInfo>()
            .Where(e => areaIdList.Contains(e.Id))
            .ToList();
        var streetList = session.Query<AddressStreetInfo>()
            .Where(e => streetIdList.Contains(e.Id))
            .ToList();

        var sendAddressItemList = sendAddressList
            .Select(e => new CustomerSendAddressListItem {
                Id = e.Id,
                Name = e.Name,
                Mobile = e.Mobile,
                Province = e.Province.Name,
                City = e.City.Name,
                Area = e.Area.Name,
                Street = e.Street.Name,
                Address = e.Address,
                Zipcode = e.Zipcode,
                IsDefault = e.IsDefault,
            })
            .ToList();

        int sendAdderssCount = sendAddressList.Count;

        int receiveAddressCount = session.Query<CustomerReceiveAddressInfo>()
            .Where(e => e.Customer.Id == loginCustomerId)
            .Count();

        return new CustomerSendAddressListModel {
            SendAddressList = sendAddressItemList,
            SendAddressCount = sendAdderssCount,
            ReceiveAddressCount = receiveAddressCount,
        };
    }

    [HttpPost]
    [Route("/client/customer/send/address/detail")]
    public IActionResult Detail([FromBody] IdParam param) {
        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        int addressId = param.Id;
        CustomerSendAddressInfo address;
        if (addressId == 0) {
            address = new CustomerSendAddressInfo();
        } else {
            address = session.Get<CustomerSendAddressInfo>(addressId);
            if (address == null) {
                throw new ValidationException($"指定的发货地址#{addressId}不存在！");
            }

            if (address.Customer.Id != loginCustomerId) {
                throw new ValidationException($"指定的发货地址#{addressId}不属于当前登录客户#{loginCustomerId}");
            }
        }

        var provinceList = session.Query<AddressProvinceInfo>().OrderBy(e => e.Sort).ToList();
        var provinceItemList = provinceList
            .Select(e => new CustomerSendAddressRegionItem {
                Value = e.Id.ToString(),
                Text = e.Name,
                Children = new List<CustomerSendAddressRegionItem>(),
            })
            .ToList();

        var model = new CustomerSendAddressDetailModel {
            Id = address.Id,
            Name = address.Name,
            Mobile = address.Mobile,
            ProvinceId = address.Province.Id,
            CityId = address.City.Id,
            AreaId = address.Area.Id,
            StreetId = address.Street.Id,
            Address = address.Address,
            Zipcode = address.Zipcode,

            ProvinceList = provinceItemList,
        };

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = model,
        });
    }

    [HttpPost]
    [Route("/client/customer/send/address/detail/region/list")]
    public IActionResult RegionList([FromBody] CustomerSendAddressCityListParam param) {
        IList<CustomerSendAddressRegionItem> provinceItemList = new List<CustomerSendAddressRegionItem>();
        if (param.TabIndex == 0) {
            int selectProvinceId = param.RegionId;
            var province = session.Get<AddressProvinceInfo>(selectProvinceId);
            if (province == null) {
                throw new ValidationException($"指定的省份#{selectProvinceId}不存在！");
            }

            var cityList = session.Query<AddressCityInfo>()
                .Where(e => e.Province.Id == selectProvinceId)
                .ToList();

            provinceItemList = new List<CustomerSendAddressRegionItem> {
                new CustomerSendAddressRegionItem {
                    Value = province.Id.ToString(),
                    Text = province.Name,
                    Children = cityList
                        .OrderBy(c => c.Sort)
                        .Select(c => new CustomerSendAddressRegionItem {
                            Value = c.Id.ToString(),
                            Text = c.Name,
                            Children = new List<CustomerSendAddressRegionItem>(),
                        }).ToList(),
                }
            };
        }

        if (param.TabIndex == 1) {
            int cityId = param.RegionId;
            var city = session.Get<AddressCityInfo>(cityId);
            if (city == null) {
                throw new ValidationException($"指定的城市#{cityId}不存在！");
            }

            NHibernateUtil.Initialize(city.Province);
            var province = city.Province;

            var areaList = session.Query<AddressAreaInfo>()
                .Where(e => e.City.Id == cityId)
                .ToList();

            provinceItemList = new List<CustomerSendAddressRegionItem> {
                new CustomerSendAddressRegionItem {
                    Value = province.Id.ToString(),
                    Text = province.Name,
                    Children = new List<CustomerSendAddressRegionItem> {
                        new CustomerSendAddressRegionItem {
                            Value = city.Id.ToString(),
                            Text = city.Name,
                            Children = areaList
                                .OrderBy(a => a.Sort)
                                .Select(a => new CustomerSendAddressRegionItem {
                                    Value = a.Id.ToString(),
                                    Text = a.Name,
                                })
                                .ToList()
                        }
                    },
                }
            };
        }

        if (param.TabIndex == 2) {
            int areaId = param.RegionId;
            var area = session.Get<AddressAreaInfo>(areaId);
            if (area == null) {
                throw new ValidationException($"指定的区/县#{areaId}不存在！");
            }
        }

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = provinceItemList,
        });
    }

    [HttpPost]
    [Route("/client/customer/send/address/save")]
    [Transactional]
    public IActionResult Save([FromBody] CustomerSendAddressSaveModel model) {
        if (ModelState.IsValid == false) {
            throw new ValidationException(ValidationUtil.GetErrorMessagesForMobile(ModelState));
        }

        int loginCustomerId = loginCustomerService.GetLoginCustomerId();
        var customer = session.Get<CustomerInfo>(loginCustomerId);
        if (customer == null) {
            throw new ValidationException($"指定的客户#{loginCustomerId}不存在！");
        }

        int addressId = model.Id;
        CustomerSendAddressInfo address;
        if (addressId == 0) {
            address = new CustomerSendAddressInfo();
            address.Customer = customer;
            address.Status = CustomerAddressStatus.New;
        } else {
            address = session.Get<CustomerSendAddressInfo>(addressId);
            if (address == null) {
                throw new ValidationException($"指定的发货地址#{addressId}不存在！");
            }

            if (address.Customer.Id != loginCustomerId) {
                throw new ValidationException($"指定的发货地址#{addressId}不属于当前登录客户#{loginCustomerId}");
            }
        }

        address.Country = Country.China;
        address.Name = model.Name;
        address.Mobile = model.Mobile;
        address.Province = session.Get<AddressProvinceInfo>(model.ProvinceId);
        address.City = session.Get<AddressCityInfo>(model.CityId);
        address.Area = session.Get<AddressAreaInfo>(model.AreaId);
        address.Street = session.Get<AddressStreetInfo>(model.StreetId);
        address.Address = model.Address;
        address.Zipcode = model.Zipcode;

        session.SaveOrUpdate(address);
        session.Flush();

        return Json(new JsonResultData {
            Success = true,
            Message = "发货地址保存成功！",
        });
    }

    [HttpPost]
    [Route("/client/customer/send/address/set/default")]
    [Transactional]
    public IActionResult SetDefault([FromBody] CustomerSendAddressSetDefaultParam param) {
        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        int addressId = param.Id;
        var address = session.Get<CustomerSendAddressInfo>(addressId);
        if (address == null) {
            throw new ValidationException($"指定的发货地址#{addressId}不存在！");
        }

        if (address.Customer.Id != loginCustomerId) {
            throw new ValidationException($"指定的发货地址#{addressId}不属于当前登录客户#{loginCustomerId}");
        }

        // 将其他的有默认标记的地址，设置成非默认地址
        // 一般应该也只有一个
        var addressDefaultList = session.Query<CustomerSendAddressInfo>()
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
            Message = "设置默认发货地址成功！",
            Data = model,
        });
    }

    [HttpPost]
    [Route("/client/customer/send/address/delete")]
    [Transactional]
    public IActionResult Delete([FromBody] CustomerSendAddressDeleteParam param) {
        if (ModelState.IsValid == false) {
            throw new ValidationException(ValidationUtil.GetErrorMessagesForMobile(ModelState));
        }

        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        int addressId = param.Id;
        var address = session.Get<CustomerSendAddressInfo>(addressId);
        if (address == null) {
            throw new ValidationException($"指定的发货地址#{addressId}不存在！");
        }

        if (address.Customer.Id != loginCustomerId) {
            throw new ValidationException($"指定的发货地址#{addressId}不属于当前登录客户#{loginCustomerId}");
        }

        session.Delete(address);
        session.Flush();

        var model = GetListData(param);

        return Json(new JsonResultData {
            Success = true,
            Message = "发货地址删除成功！",
            Data = model,
        });
    }
}