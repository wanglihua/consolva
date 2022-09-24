using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Pass;

namespace shunda.Admin.Areas.Pass.Models.GroupPassRes;

public class GroupPassResIndexModel
{
    [BindNever]
    public int GroupId { get; set; }

    [BindNever]
    public GroupInfo Group { get; set; } = new GroupInfo();
}