using Microsoft.AspNetCore.Mvc;
using shunda.Admin.Areas.Customer.Models.CustomerScoreRule;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Customer;
using shunda.Domain.Pass.PassRes;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Customer.Controllers;

[CustomerArea]
[AdminAuth(PassRes.Customer.CustomerScoreRule)]
public class CustomerScoreRuleController : Controller
{
    private readonly ISession session;

    public CustomerScoreRuleController(ISession session)
    {
        this.session = session;
    }

    [HttpGet]
    [Route("/admin/customer/score/rule")]
    public IActionResult Index()
    {
        var model = new CustomerScoreRuleIndexModel();

        var ruleList = session.Query<CustomerScoreRuleInfo>().ToList();

        // Promotion
        var promotionRule = ruleList
            .Where(e => e.Type == CustomerScoreType.Promotion)
            .OrderBy(e => e.Id)
            .Take(1)
            .SingleOrDefault();

        if (promotionRule == null)
        {
            promotionRule = new CustomerScoreRuleInfo();
            promotionRule.Type = CustomerScoreType.Promotion;
        }

        model.PromotionRule = promotionRule;

        // LogisticsConsume
        var logisticsConsumeRule = ruleList
            .Where(e => e.Type == CustomerScoreType.LogisticsConsume)
            .OrderBy(e => e.Id)
            .Take(1)
            .SingleOrDefault();

        if (logisticsConsumeRule == null)
        {
            logisticsConsumeRule = new CustomerScoreRuleInfo();
            logisticsConsumeRule.Type = CustomerScoreType.LogisticsConsume;
        }

        model.LogisticsConsumeRule = logisticsConsumeRule;

        // ScoreToBalance
        var scoreToBalanceRule = ruleList
            .Where(e => e.Type == CustomerScoreType.ScoreToBalance)
            .OrderBy(e => e.Id)
            .Take(1)
            .SingleOrDefault();

        if (scoreToBalanceRule == null)
        {
            scoreToBalanceRule = new CustomerScoreRuleInfo();
            scoreToBalanceRule.Type = CustomerScoreType.ScoreToBalance;
        }

        model.ScoreToBalanceRule = scoreToBalanceRule;

        return View("CustomerScoreRuleIndex", model);
    }

    [HttpPost]
    [Route("/admin/customer/score/rule/save")]
    [Transactional]
    public IActionResult Save(CustomerScoreRuleIndexModel model)
    {
        if (!ModelState.IsValid) throw new ValidationException(ValidationUtil.GetErrorMessages(ModelState));

        var ruleList = session.Query<CustomerScoreRuleInfo>().ToList();

        // Promotion
        var promotionRule = ruleList
            .Where(e => e.Type == CustomerScoreType.Promotion)
            .OrderBy(e => e.Id)
            .Take(1)
            .SingleOrDefault();

        if (promotionRule == null)
        {
            promotionRule = new CustomerScoreRuleInfo();
            promotionRule.Type = CustomerScoreType.Promotion;
        }

        promotionRule.Amount = model.PromotionRule.Amount;
        promotionRule.Score = model.PromotionRule.Score;

        session.SaveOrUpdate(promotionRule);

        // LogisticsConsume
        var logisticsConsumeRule = ruleList
            .Where(e => e.Type == CustomerScoreType.LogisticsConsume)
            .OrderBy(e => e.Id)
            .Take(1)
            .SingleOrDefault();

        if (logisticsConsumeRule == null)
        {
            logisticsConsumeRule = new CustomerScoreRuleInfo();
            logisticsConsumeRule.Type = CustomerScoreType.LogisticsConsume;
        }

        logisticsConsumeRule.Amount = model.LogisticsConsumeRule.Amount;
        logisticsConsumeRule.Score = model.LogisticsConsumeRule.Score;

        session.SaveOrUpdate(logisticsConsumeRule);

        // ScoreToBalance
        var scoreToBalanceRule = ruleList
            .Where(e => e.Type == CustomerScoreType.ScoreToBalance)
            .OrderBy(e => e.Id)
            .Take(1)
            .SingleOrDefault();

        if (scoreToBalanceRule == null)
        {
            scoreToBalanceRule = new CustomerScoreRuleInfo();
            scoreToBalanceRule.Type = CustomerScoreType.ScoreToBalance;
        }

        scoreToBalanceRule.Amount = model.ScoreToBalanceRule.Amount;
        scoreToBalanceRule.Score = model.ScoreToBalanceRule.Score;

        session.SaveOrUpdate(scoreToBalanceRule);

        session.Flush();

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "message", "保存成功！" }
        });
    }
}