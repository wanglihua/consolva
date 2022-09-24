using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Pass;

namespace shunda.Admin.Areas.Pass.Models.PassResUser;

public class PassResUserIndexModel
{
    [BindNever]
    public int PassResId { get; set; }

    [BindNever]
    public PassResInfo PassRes { get; set; } = new PassResInfo();
}