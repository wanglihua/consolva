using shunda.Domain.Customer;

namespace shunda.Admin.Areas.Customer.Models.CustomerScoreRule;

public class CustomerScoreRuleIndexModel
{
    public CustomerScoreRuleInfo PromotionRule { get; set; } = new();

    public CustomerScoreRuleInfo LogisticsConsumeRule { get; set; } = new();

    public CustomerScoreRuleInfo ScoreToBalanceRule { get; set; } = new();
}