using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Customer;

namespace shunda.Admin.Areas.Customer.Models.CustomerScoreLog;

public class CustomerScoreLogIndexModel
{
    [BindNever]
    public IList<CustomerScoreType> ScoreTypeList { get; set; } = CustomerScoreType.GetList();
}