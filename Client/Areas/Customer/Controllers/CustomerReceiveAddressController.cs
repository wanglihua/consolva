using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Customer.Models.CustomerReceiveAddress;
using shunda.Client.Services;
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
        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new CustomerReceiveAddressListModel {
            },
        });
    }
}