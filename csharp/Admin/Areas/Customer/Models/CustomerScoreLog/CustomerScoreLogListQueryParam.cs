using Microsoft.AspNetCore.Mvc;
using shunda.Core;
using shunda.Domain.Customer;

namespace shunda.Admin.Areas.Customer.Models.CustomerScoreLog;

public class CustomerScoreLogListQueryParam : ListQueryParam
{
    public string? CustomerCodeQuery { get; set; }

    [ModelBinder(typeof(CustomerScoreTypeModelBinder))]
    public CustomerScoreType? ScoreTypeQuery { get; set; }

    public string? OrderCodeQuery { get; set; }
}