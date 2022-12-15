using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Customer.Models.CustomerSendAddress;
using shunda.Client.Services;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Customer;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Customer.Controllers;

[CustomerArea]
public class CustomerSendAddressController : Controller
{
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;

    public CustomerSendAddressController(ISession session, LoginCustomerService loginCustomerService)
    {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
    }

    [HttpPost]
    [Route("/client/customer/send/address/list")]
    public IActionResult List([FromBody] CustomerSendAddressListParam param)
    {
        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        var model = GetListData(param);

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = model,
        });
    }

    private CustomerSendAddressListModel GetListData(CustomerSendAddressListParam param)
    {
        return new CustomerSendAddressListModel();
    }

    [HttpPost]
    [Route("/client/customer/send/address/detail")]
    public IActionResult Detail([FromBody] IdParam param)
    {
        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new CustomerSendAddressDetailModel {
            },
        });
    }

    [HttpPost]
    [Route("/client/customer/send/address/save")]
    [Transactional]
    public IActionResult Save([FromBody] CustomerSendAddressSaveModel model)
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
            var address = session.Get<CustomerSendAddressInfo>(addressId);
            if (address == null)
            {
                throw new ValidationException($"指定的发货地址#{addressId}不存在！");
            }

            if (address.Customer.Id != loginCustomerId)
            {
                throw new ValidationException($"指定的发货地址#{addressId}不属于当前登录客户#{loginCustomerId}");
            }
        }

        return Json(new JsonResultData {
            Success = true,
            Message = "发货地址保存成功！",
        });
    }

    [HttpPost]
    [Route("/client/customer/send/address/set/default")]
    [Transactional]
    public IActionResult SetDefault([FromBody] CustomerSendAddressSetDefaultParam param)
    {
        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        int addressId = param.Id;
        var address = session.Get<CustomerSendAddressInfo>(addressId);
        if (address == null)
        {
            throw new ValidationException($"指定的发货地址#{addressId}不存在！");
        }

        if (address.Customer.Id != loginCustomerId)
        {
            throw new ValidationException($"指定的发货地址#{addressId}不属于当前登录客户#{loginCustomerId}");
        }

        // 将其他的有默认标记的地址，设置成非默认地址
        // 一般应该也只有一个
        var addressDefaultList = session.Query<CustomerSendAddressInfo>()
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
            Message = "设置默认发货地址成功！",
            Data = model,
        });
    }

    [HttpPost]
    [Route("/client/customer/send/address/delete")]
    [Transactional]
    public IActionResult Delete([FromBody] CustomerSendAddressDeleteParam param)
    {
        if (ModelState.IsValid == false)
        {
            throw new ValidationException(ValidationUtil.GetErrorMessagesForMobile(ModelState));
        }

        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        int addressId = param.Id;
        var address = session.Get<CustomerSendAddressInfo>(addressId);
        if (address == null)
        {
            throw new ValidationException($"指定的发货地址#{addressId}不存在！");
        }

        if (address.Customer.Id != loginCustomerId)
        {
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

    // 详细，编辑，删除时 检查是不是当前登录客户的地址
    // 删除 和 设置默认地址 后，直接返回地址列表
}