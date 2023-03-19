using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Customer.Models.CustomerAccountCharge;
using shunda.Core.Web;
using shunda.Domain.Fee;

namespace shunda.Client.Areas.Customer.Controllers;

[CustomerArea]
public class CustomerAccountChargeController : Controller {
    [HttpPost]
    [Route("/client/customer/account/charge")]
    public IActionResult Index() {
        var currencyList = CurrencyType.GetList()
            .Where(e => e != CurrencyType.CNY)
            .ToList();

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new CustomerAccountChargeIndexModel {
                CurrencyList = currencyList,
            },
        });
    }
}