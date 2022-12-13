using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Account.Models.CustomerScore;
using shunda.Client.Services;
using shunda.Core;
using shunda.Core.Web;
using shunda.Domain.Customer;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Account.Controllers;

[AccountArea]
public class CustomerScoreController : Controller
{
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;

    public CustomerScoreController(ISession session, LoginCustomerService loginCustomerService)
    {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
    }

    [HttpPost]
    [Route("/client/customer/score")]
    public IActionResult Index()
    {
        int loginCustomerId = loginCustomerService.GetLoginCustomerId();
        var customer = session.Get<CustomerInfo>(loginCustomerId);
        if (customer == null)
        {
            throw new ValidationException($"指定的客户#{loginCustomerId}不存在！");
        }

        string totalScore = customer.TotalScore.ToString("#0.00");
        string validScore = customer.ValidScore.ToString("#0.00");

        string vipRank = customer.Level.Name; // 普通会员，超级VIP
        if (customer.Level == CustomerLevel.Vip)
        {
            vipRank = customer.VipRank.Name; // VIP1, VIP2,...
        }

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new CustomerScoreIndexModel {
                TotalScore = totalScore,
                ValidScore = validScore,
                VipRank = vipRank,
            }
        });
    }
}