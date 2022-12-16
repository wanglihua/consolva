using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Customer.Models.CustomerScore;
using shunda.Client.Services;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Customer;
using shunda.Service.Customer;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Customer.Controllers;

[CustomerArea]
public class CustomerScoreController : Controller
{
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;
    private readonly CustomerScoreService customerScoreService;

    public CustomerScoreController(ISession session, LoginCustomerService loginCustomerService, CustomerScoreService customerScoreService)
    {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
        this.customerScoreService = customerScoreService;
    }

    [HttpPost]
    [Route("/client/customer/score")]
    public IActionResult Index()
    {
        int loginCustomerId = loginCustomerService.GetLoginCustomerId();
        var model = GetCustomerScoreData(loginCustomerId);

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = model,
        });
    }

    [NonAction]
    private CustomerScoreIndexModel GetCustomerScoreData(int customerId)
    {
        var customer = session.Get<CustomerInfo>(customerId);
        if (customer == null)
        {
            throw new ValidationException($"指定的客户#{customerId}不存在！");
        }

        string totalScore = customer.TotalScore.ToString("#0.00");
        string validScore = customer.ValidScore.ToString("#0.00");

        string vipRank = customer.Level.Name; // 普通会员，超级VIP
        if (customer.Level == CustomerLevel.Vip)
        {
            vipRank = customer.VipRank.Name; // VIP1, VIP2,...
        }

        return new CustomerScoreIndexModel {
            TotalScore = totalScore,
            ValidScore = validScore,
            VipRank = vipRank,
        };
    }

    [HttpPost]
    [Route("/client/customer/score/to/account")]
    [Transactional]
    public IActionResult ScoreToAccount([FromBody] CustomerScoreToAccountModel model)
    {
        if (!this.ModelState.IsValid)
        {
            throw new ValidationException(ValidationUtil.GetErrorMessagesForMobile(this.ModelState));
        }

        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        decimal score = model.Score;

        decimal amount = customerScoreService.ScoreToAccount(loginCustomerId, score);

        var indexModel = GetCustomerScoreData(loginCustomerId);

        return Json(new JsonResultData {
            Success = true,
            Message = $"兑换成功，兑换积分 {score.ToString("#0.00")}，换得余额 {amount.ToString("#0.00")}！",
            Data = indexModel,
        });
    }
}