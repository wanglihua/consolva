using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Customer.Models.CustomerReceiveAddress;
using shunda.Client.Services;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Customer;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Customer.Controllers;

[CustomerArea]
public class CustomerReceiveAddressController : Controller
{
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;

    public CustomerReceiveAddressController(ISession session, LoginCustomerService loginCustomerService)
    {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
    }

    [HttpPost]
    [Route("/client/customer/receive/address/list")]
    public IActionResult List([FromBody] CustomerReceiveAddressListParam param)
    {
        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        var model = GetListData(param);

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = model,
        });
    }

    private CustomerReceiveAddressListModel GetListData(CustomerReceiveAddressListParam param)
    {
        return new CustomerReceiveAddressListModel();
    }

    [HttpPost]
    [Route("/client/customer/receive/address/detail")]
    public IActionResult Detail([FromBody] IdParam param)
    {
        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new CustomerReceiveAddressDetailModel {
            },
        });
    }

    [HttpPost]
    [Route("/client/customer/receive/address/save")]
    [Transactional]
    public IActionResult Save([FromBody] CustomerReceiveAddressSaveModel model)
    {
        if (ModelState.IsValid == false)
        {
            throw new ValidationException(ValidationUtil.GetErrorMessagesForMobile(ModelState));
        }

        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        int addressId = model.Id;
        if (addressId == 0)
        {
        }
        else
        {
            var address = session.Get<CustomerReceiveAddressInfo>(addressId);
            if (address == null)
            {
                throw new ValidationException($"指定的收货地址#{addressId}不存在！");
            }

            if (address.Customer.Id != loginCustomerId)
            {
                throw new ValidationException($"指定的收货地址#{addressId}不属于当前登录客户#{loginCustomerId}");
            }
        }

        return Json(new JsonResultData {
            Success = true,
            Message = "收货地址保存成功！",
        });
    }

    [HttpPost]
    [Route("/client/customer/receive/address/set/default")]
    [Transactional]
    public IActionResult SetDefault([FromBody] CustomerReceiveAddressSetDefaultParam param)
    {
        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        int addressId = param.Id;
        var address = session.Get<CustomerReceiveAddressInfo>(addressId);
        if (address == null)
        {
            throw new ValidationException($"指定的收货地址#{addressId}不存在！");
        }

        if (address.Customer.Id != loginCustomerId)
        {
            throw new ValidationException($"指定的收货地址#{addressId}不属于当前登录客户#{loginCustomerId}");
        }

        // 将其他的有默认标记的地址，设置成非默认地址
        // 一般应该也只有一个
        var addressDefaultList = session.Query<CustomerReceiveAddressInfo>()
            .Where(e => e.Customer.Id == loginCustomerId && e.IsDefault == true)
            .ToList();
        foreach (var addressDefault in addressDefaultList)
        {
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
    public IActionResult Delete([FromBody] CustomerReceiveAddressDeleteParam param)
    {
        if (ModelState.IsValid == false)
        {
            throw new ValidationException(ValidationUtil.GetErrorMessagesForMobile(ModelState));
        }

        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        int addressId = param.Id;
        var address = session.Get<CustomerReceiveAddressInfo>(addressId);
        if (address == null)
        {
            throw new ValidationException($"指定的收货地址#{addressId}不存在！");
        }

        if (address.Customer.Id != loginCustomerId)
        {
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

    // 详细，编辑，删除时 检查是不是当前登录客户的地址
    // 删除 和 设置默认地址 后，直接返回地址列表
}