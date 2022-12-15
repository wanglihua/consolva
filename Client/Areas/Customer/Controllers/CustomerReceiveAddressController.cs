using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Customer.Models.CustomerReceiveAddress;
using shunda.Client.Services;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
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

        return Json(new JsonResultData {
            Success = true,
            Message = "收货地址保存成功！",
        });
    }

    [HttpPost]
    [Route("/client/customer/receive/address/set/default")]
    public IActionResult SetDefault([FromBody] CustomerReceiveAddressSetDefaultParam param)
    {
        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

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