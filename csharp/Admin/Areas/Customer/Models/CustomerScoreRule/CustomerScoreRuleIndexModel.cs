using shunda.Domain.Customer;

namespace shunda.Admin.Areas.Customer.Models.CustomerScoreRule;

public class CustomerScoreRuleIndexModel
{
    public CustomerScoreRuleInfo PromotionRule { get; set; } = new CustomerScoreRuleInfo();

    public CustomerScoreRuleInfo LogisticsConsumeRule { get; set; } = new CustomerScoreRuleInfo();

    public CustomerScoreRuleInfo ScoreToBalanceRule { get; set; } = new CustomerScoreRuleInfo();
}