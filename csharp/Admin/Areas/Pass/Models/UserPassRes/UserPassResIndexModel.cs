using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Pass;

namespace shunda.Admin.Areas.Pass.Models.UserPassRes;

public class UserPassResIndexModel
{
    public int UserId { get; set; }

    [BindNever]
    public UserInfo User { get; set; } = new UserInfo();
}