using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Pass;

namespace shunda.Admin.Areas.Pass.Models.PassResGroup;

public class PassResGroupIndexModel
{
    [BindNever]
    public int PassResId { get; set; }

    [BindNever]
    public PassResInfo PassRes { get; set; } = new();
}