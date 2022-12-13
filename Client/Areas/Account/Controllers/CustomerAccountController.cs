using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Account.Models.CustomerAccount;
using shunda.Client.Services;
using shunda.Core;
using shunda.Core.Web;
using shunda.Domain.Customer;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Account.Controllers;

[AccountArea]
public class CustomerAccountController : Controller
{
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;

    public CustomerAccountController(ISession session, LoginCustomerService loginCustomerService)
    {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
    }

    [HttpPost]
    [Route("/client/customer/account")]
    public IActionResult Index()
    {
        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        var customer = session.Get<CustomerInfo>(loginCustomerId);
        if (customer == null)
        {
            throw new ValidationException($"指定的客户#{loginCustomerId}不存在！");
        }

        var amount = customer.Account;

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new CustomerAccountIndexModel {
                Amount = amount.ToString("#0.00"),
            }
        });
    }
}