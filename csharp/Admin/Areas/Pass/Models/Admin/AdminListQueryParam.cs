using System;
using shunda.Core;

namespace shunda.Admin.Areas.Pass.Models.Admin;

public class AdminListQueryParam : ListQueryParam
{
    public string? LoginNameQuery { get; set; }

    public string? UserNameQuery { get; set; }
}